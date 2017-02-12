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
            return new ServiceReplicaListener[0];
        }

        public async Task<ValidationResult> ValidateEntity<T>(T entity)
        {
            ValidationResult rtn = null;
            //var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<ComparableType, IGoodVibesValidator>>(ValidatorDictionary);
            var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<ComparableType, List<XElement>>>(SerializedValidatorDictionary);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var conditionalValidator = await validators.TryGetValueAsync(tx, new ComparableType(typeof(T)));
                if (conditionalValidator.HasValue)
                {
                     var validator = (IGoodVibesValidatorFor<T>)conditionalValidator.Value;
                    rtn = validator.Validate(entity);
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
            var validators = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, List<XElement>>>(SerializedValidatorDictionary);
            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await validators.GetCountAsync(tx) == 0)
                {
                    Expression<Func<ReportBase, ValidationMessage>> ratingNotNull = x => new ValidationMessage()
                    {
                        Success = x.Rating != Ratings.None,
                        Name = "Rating populated",
                        ErrorMessage = (x.Rating != Ratings.None) ? "" : "A rating must be entered"
                        
                    } ;
                    XElement serializedValidator = _serializer.Serialize(ratingNotNull);
                    List<XElement> values = new List<XElement>();
                    values.Add(serializedValidator);

                    await validators.AddAsync(tx, 1, values);
                }
                await tx.CommitAsync();
            }
            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await validators.GetCountAsync(tx) != 0)
                {
                    var val = await validators.TryGetValueAsync(tx, 1);
                    if (val.HasValue)
                    {
                        XElement x = val.Value.FirstOrDefault();
                        var serializedValidator = _serializer.Deserialize<Func<ReportBase, ValidationMessage>>(x);
                        var test = new SurfReport(Ratings.None, "ben", "dfs", DateTime.Now, null, 0, 0);
                        var godIHopeThisJustFuckingWorksForOnce = serializedValidator.Compile()(test);

                    }

                }

            }

        }

        public async Task<ValidationResult> ValidateSnurfReport(ReportBase report)
        {
            return await this.ValidateEntity<ReportBase>(report);
        }

        public async Task<ValidationResult> ValidateSurfReport(SurfReport report)
        {
            return await this.ValidateEntity<SurfReport>(report);
        }

        public async Task<ValidationResult> ValidateSnowReport(SnowReport report)
        {
            return await this.ValidateEntity<SnowReport>(report);
        }
    }
}
