using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ValidationService.Contracts;
using ValidationService.Interfaces;
using SnurfReportService.Interfaces;
using ValidationService.Validators;
using Microsoft.ServiceFabric.Data;
using ExpressionSerialization;
using System.Xml.Linq;
using System.Linq.Expressions;
using ValidationService.Seed;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Diagnostics;

namespace ValidationService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class ValidationService : StatefulService, ISnurfReportValidationService
    {
        public const string ValidatorDictionary = "Validators";
        public const string SerializedValidatorDictionary = "SerializedValidators";
        private ExpressionSerializer _serializer;
        public ValidationService(StatefulServiceContext context)
            : base(context)
        { }
        public ValidationService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica)
        : base(serviceContext, reliableStateManagerReplica)
    {
        }
        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener>() {
                new ServiceReplicaListener((context) =>
               this.CreateServiceRemotingListener(context))
            };
        }

        public async Task<ValidationResult> ValidateEntity<T>(T entity)
        {
            ValidationResult rtn = new ValidationResult();
            //var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<ComparableType, IGoodVibesValidator>>(ValidatorDictionary);
            var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<XElement>>>(SerializedValidatorDictionary);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var conditionalValidatorRules = await validators.TryGetValueAsync(tx, typeof(T).AssemblyQualifiedName);
                if (conditionalValidatorRules.HasValue)
                    {
                        foreach (var xml in conditionalValidatorRules.Value)
                        {
                            try
                            {
                                var x = _serializer.Deserialize<Func<T, ValidationMessage>>(xml);
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError(ex.ToString());
                            }

                        }
                         var validationRules = conditionalValidatorRules.Value.Select(x => _serializer.Deserialize<Func<T, ValidationMessage>>(x)).ToList();
                        var typeValidator = GoodVibesValidator<T>.Create(validationRules);
                        var result = typeValidator.Validate(entity);
                        rtn.AddValidationResults(result);
                    }

            }
            return rtn;
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _serializer = Serialization.CreateSerializer();

            //This is essentially mapping types to objects. The IGoodVibesValidator just enforces that you use the interface for some semblance
            //of control. That said, you can quite eaisily break this if you are so inclined
            //var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<ComparableType, IGoodVibesValidator>>(ValidatorDictionary);
            var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<XElement>>>(SerializedValidatorDictionary);
            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await validators.GetCountAsync(tx) == 0)
                {
                    List<XElement> values = new List<XElement>();
                    //Expression<Func<ReportBase, ValidationMessage>> expr1 = ReportValidationRules.RatingRule;
                    //Expression<Func<ReportBase, ValidationMessage>> expr2 = ReportValidationRules.PosterRule;
                    //Expression<Func<ReportBase, ValidationMessage>> expr3 = ReportValidationRules.LocationRule;

                    //values.Add(_serializer.Serialize(expr1));
                    //values.Add(_serializer.Serialize(expr2));
                    //values.Add(_serializer.Serialize(expr3));

                    ReportValidationRules.ReportRules.ForEach(x => values.Add(_serializer.Serialize(x)));
                    await validators.AddAsync(tx, typeof(ReportBase).AssemblyQualifiedName, values);
                }
                await tx.CommitAsync();
            }
            //using (var tx = this.StateManager.CreateTransaction())
            //{
            //    if (await validators.GetCountAsync(tx) != 0)
            //    {
            //        var val = await validators.TryGetValueAsync(tx, typeof(SurfReport).AssemblyQualifiedName);
            //        if (val.HasValue)
            //        {
            //            XElement x = val.Value.FirstOrDefault();
            //            var serializedValidator = _serializer.Deserialize<Func<ReportBase, ValidationMessage>>(x);
            //            var test = new SurfReport(Ratings.None, "ben", "dfs", DateTime.Now, null, 0, 0);
            //            var godIHopeThisJustFuckingWorksForOnce = serializedValidator.Compile()(test);

            //        }

            //    }

            //}

        }

        public async Task<ValidationResult> ValidateSnurfReport(ReportBase report)
        {
            return await this.ValidateEntity<ReportBase>(report);
        }

        public async Task<ValidationResult> ValidateSurfReport(SurfReport report)
        {
            ValidationResult rtn = new ValidationResult();
            //There may be a better way to do this, but the generic arguments required from the serialization library don't allow you to pass in a more derived type and get 
            //return type covariance. Although I always seem to run into this problem in C#.... So, we need to manually validate against the known base types.
            var baseResult = this.ValidateEntity<ReportBase>(report as ReportBase);
            var coreResult = this.ValidateEntity<SurfReport>(report);
            //These two tasks can be run in parallel. So do it.
            await Task.WhenAll(baseResult, coreResult);
            //I don't really know how to use a synchronous continue with call, so let's just force synchrocity this way
            rtn.AddValidationResults(new ValidationResult[2] { baseResult.Result, coreResult.Result });
            return rtn;
        }

        public async Task<ValidationResult> ValidateSnowReport(SnowReport report)
        {
            //see comments above
            ValidationResult rtn = new ValidationResult();
            var baseResult = this.ValidateEntity<ReportBase>(report as ReportBase);
            var coreResult = this.ValidateEntity<SnowReport>(report);
            await Task.WhenAll(baseResult, coreResult);
            rtn.AddValidationResults(new ValidationResult[2] { baseResult.Result, coreResult.Result });
            return rtn;
        }
    }
}
