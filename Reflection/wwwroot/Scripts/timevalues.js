var timearray = [];
var timestring = '';
var optiondata = '';
$(document).ready(function () {
    var presentdate = new Date();
    var minutes = presentdate.getMinutes();
    if (minutes < 30) {
        presentdate.setMinutes(30)
    }
    else {
        presentdate.setHours(presentdate.getHours() + 1)
        presentdate.setMinutes(00)
    }
    var nextdate = new Date(presentdate.toISOString());
    nextdate.setDate(nextdate.getDate() + 1);
    while (presentdate < nextdate) {
        timestring = timestring + (presentdate.getHours() == '0' ? '00' : presentdate.getHours());
        timestring = timestring + ":";
        timestring = timestring + (presentdate.getMinutes() == '0' ? '00' : presentdate.getMinutes());
        timearray.push(timestring);
        presentdate.setMinutes(presentdate.getMinutes() + 30);
        timestring = '';
    }
    exectime
    timearray.forEach(x => {
        var time = '';
        if (x.split(':')[0] > 12) {
            var timearray = x.split(':');
            timearray[0] = timearray[0] - 12;
            time = timearray.join(':') + ' PM'
        }
        else {
            time = x + ' AM'
        }
        optiondata = optiondata + '<option val="' + time + '">' + time + '</option>';
    });
    $("#exectime").append(optiondata);
});
