var serviceUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');

$(function () {
    $( "#turnOffSnowService").click(function() {
        TurnOffSnowService();
    });
    $( "#getSnowServiceStatus").click(function() {
        GetSnowConditionsServiceStatus();
    });
    $( "#getSnowPack").click(function() {
        GetSnowConditions();
    });
});

function TurnOffSnowService() {
    $.ajax({
        url: serviceUrl + '/api/snowservice',
        dataType: 'json',
        method: 'POST',
        contentType: 'application/json',
        processData: false,
        data: JSON.stringify({ "TurnServiceOn": false })
    })
   .success(function (response) {

   });
}

function GetSnowConditionsServiceStatus() {
    $.ajax({
        url: serviceUrl + '/api/snowservice',
        dataType: 'json',
        method: 'GET'
    })
   .done(function (response) {
       alert(response.toString());
   });
}
function GetSnowConditions() {
    $("#waitingForResponse").fadeIn();
    $.ajax({
        url: serviceUrl + '/api/snowconditions',
        dataType: 'json',
        method: 'GET',
        success: function (getConditionsResponse) {
            $("#waitingForResponse").fadeOut();
            if (getConditionsResponse.hasOwnProperty("Snowpack")) {
                $('#snowpack').val(getConditionsResponse.Snowpack.toString())
            }
        },
        error: function (error) {
            $("#waitingForResponse").fadeOut();
            $("#responseFailed").fadeIn();
            console.log(error);
        }
    });
}
