//js file to access data from snow conditions and update any corresponding elements. right now this is all manually managed hardcore brute force jqurey way, but if someone wants to 
//be a baws and implement angular2 (since that is the defacto fpo standard) then be my guest. I just don't need another fucking project

var serviceUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
var serviceIsOn = true;
$(function () {
    var getSnowfallTask = setInterval(function () { GetSnowConditions(); }, 20000);
});
function GetSnowConditions() {
    $.ajax({
        url: serviceUrl + '/api/snowconditions',
        dataType: 'json',
        method: 'GET'
    })
   .done(function (getConditionsResponse) {
       if (getConditionsResponse.hasOwnProperty("NewSnow")) {
           $('#newSnow').html(getConditionsResponse.NewSnow.toString())
       }
       if (getConditionsResponse.hasOwnProperty("Snowpack")) {
           $('#snowpack').html(getConditionsResponse.Snowpack.toString())
       }
   });
}
