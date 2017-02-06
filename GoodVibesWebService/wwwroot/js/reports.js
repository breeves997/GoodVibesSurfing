//js file to access data from snow conditions and update any corresponding elements. right now this is all manually managed hardcore brute force jqurey way, but if someone wants to 
//be a baws and implement angular2 (since that is the defacto fpo standard) then be my guest. I just don't need another fucking project

var serviceUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
$(document).ready(function () {
    //state vars
    var surfFormVisible = false;
    var snowFormVisible = false;

    $('#dailySurfReports').click(function (e) {
        GetDailySurfReports();
        e.preventDefault();
    });
    $('#dailySnowReports').click(function (e) {
        GetDailySnowReports();
        e.preventDefault();
    });
    $('#create-surf-report').click(function(e) {
        if (surfFormVisible) {
            $('#surf-report').slideUp();
        }
        else {
            $('#surf-report').slideDown();
        }
    });
    $('#create-snow-report').click(function(e) {
        if (snowFormVisible) {
            $('#snow-report').fadeOut();
        }
        else {
            $('#snow-report').fadeIn();
        }
    });
    $('#snow-submit').click(function(e) {
        PostSnowReport();
    });
    $('#surf-submit').click(function(e) {
        PostSurfReport();
    });
    $('#surf-search').click(function(e) {
        GetSurfReportByDateAndUser();
    });
    $('#snow-search').click(function(e) {
        GetSnowReportByDateAndUser();
    });
});

function BuildDate(date) {
    var dd = date.getDate();
    var mm = date.getMonth()+1; //January is 0!

    var yyyy = date.getFullYear();
    if(dd<10){
        dd='0'+dd;
    } 
    if(mm<10){
        mm='0'+mm;
    } 
    return mm+'-'+dd+'-'+yyyy;

}
function GetDailySurfReports() {
    var dateString = BuildDate(new Date());
    $.ajax({
        url: serviceUrl + '/api/surfreport?date=' + dateString,
        dataType: 'json',
        method: 'GET'
    })
   .done(function (data) { //should have standard response contract, but I'm too lazy
        $('#surfReports tbody tr').remove();
        var html = '';
        $('#surfReports tbody').append(BuildSurfTable(data));
   })
    .fail(function (response) {
        console.log(response);
    });;
}
function GetDailySnowReports() {
    var dateString = BuildDate(new Date());
    $.ajax({
        url: serviceUrl + '/api/snowreport?date=' + dateString,
        dataType: 'json',
        method: 'GET'
    })
   .done(function (data) { //should have standard response contract, but I'm too lazy
        $('#snowReports tbody tr').remove();
        $('#snowReports tbody').append(BuildSnowTable(data));
   })
    .fail(function (response) {
        console.log(response);
    });;
}

function GetSnowReportByDateAndUser() {
    var date = $('#snow-date-search').val();
    var name = $('#snow-name-search').val();
    $.ajax({
        url: serviceUrl + '/api/snowreport?date=' + date + '&poster='+name,
        dataType: 'json',
        method: 'GET'
    })
   .done(function (data) { //should have standard response contract, but I'm too lazy
       if (data.length) {
           var rpt = data[0];
           $('#snow-search-rating').val(rpt.rating)
           $('#snow-search-location').val(rpt.location)
           $('#snow-search-visibility').val(rpt.visibility)
           $('#snow-search-temperature').val(rpt.temperature)
       }

   })
    .fail(function (response) {
        console.log(response);
    });;
}

function GetSurfReportByDateAndUser() {
    var date = $('#surf-date-search').val();
    var name = $('#surf-name-search').val();
    $.ajax({
        url: serviceUrl + '/api/surfreport?date=' + date + '&poster='+name,
        dataType: 'json',
        method: 'GET'
    })
   .done(function (data) { //should have standard response contract, but I'm too lazy
       if (data.length) {
           var rpt = data[0];
           $('#surf-search-rating').val(rpt.rating)
           $('#surf-search-location').val(rpt.location)
           $('#surf-search-wave-size').val(rpt.waveSize)
           $('#surf-search-period').val(rpt.period)
       }

   })
    .fail(function (response) {
        console.log(response);
    });;
}

function BuildSurfTable(data) {
    var html = $.map(data, function (report, index) {
        return '<tr><td>' + report.rating + '</td><td>' + report.poster + '</td>' +
        '<td>' + report.location + '</td><td>' + report.date + '</td>' +
        '<td>' + report.waveSize + '</td><td>' + report.period + '</td></tr>'
    }).join('')
    return html;
}
function BuildSnowTable(data) {
    var html = $.map(data, function (report, index) {
        return '<tr><td>' + report.rating + '</td><td>' + report.poster + '</td>' +
        '<td>' + report.location + '</td><td>' + report.date + '</td>' +
        '<td>' + report.visibility + '</td><td>' + report.temperature + '</td></tr>'
    }).join('')
    return html;
}

function PostSnowReport() {
    var data = BuildSnowReport();
    $.ajax({
        url: serviceUrl + '/api/snowreport',
        dataType   : 'json',
        contentType: 'application/json; charset=UTF-8', // This is the money shot
        method: 'POST',
        data: JSON.stringify(data)
    })
   .done(function (data) { //should have standard response contract, but I'm too lazy
       GetDailySnowReports();
   })
    .fail(function (response) {
        console.log(response);
    });;

}
function PostSurfReport() {
    var data = BuildSurfReport();
    $.ajax({
        url: serviceUrl + '/api/surfreport',
        dataType   : 'json',
        contentType: 'application/json; charset=UTF-8', // This is the money shot
        method: 'POST',
        data: JSON.stringify(data)
    })
   .done(function (data) { //should have standard response contract, but I'm too lazy
       GetDailySurfReports();
   })
    .fail(function (response) {
        console.log(response);
    });;

}

function BuildSnowReport() {
    var data = {
        "attachments" : [],
        "date" : "",
        "location" : "",
        "poster" : "",
        "rating" : 0,
        "temperature" : 0,
        "visibility" : "",
    }
    data.location = $('#snow-location').val();
    data.poster = $('#snow-poster').val();
    data.rating = $('#snow-rating').val();
    data.temperature = $('#temperature').val();
    data.visibility = $('#visibility').val();
    data.date = $('#snow-date').val();
    return data;
}

    function BuildSurfReport() {
        var data = {
            "attachments" : [],
            "date" : "",
            "location" : "",
            "poster" : "",
            "rating" : 0,
            "waveSize" : 0,
            "period" : 0,
        }
        data.location = $('#surf-location').val();
        data.poster = $('#surf-poster').val();
        data.rating = $('#surf-rating').val();
        data.waveSize = $('#wave-size').val();
        data.period = $('#period').val();
        data.date = $('#surf-date').val();
        return data;
    }