/// <reference path="index.js" />
let blockdata = "";
let deleteid = "";
let editid = "";
let weeks = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
$(document).ready(function () {
    $(".loader").show();
    $("#deleteIcon").hide();
    $("#edit").hide();
    let today = moment().format("YYYY-MM-DD");
    $("#execdate").val(today);
    $("#startdatedisplay").html(today);
    $("#customnumber").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
    $('[data-toggle="popover"]').popover();
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            let head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/ManageRecurringPosts.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/ManageRecurringPosts-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/ManageRecurringPosts-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
    });
    getRecurssions();
});

$("tbody#tablebody").on("click", "td.date-day", function () {
    $('#week').toggle();
});

$('.delete-icon').click(function () {
    $('#deleteIcon').modal('show');
});

$('.edit-icon').click(function () {
    $('#edit').modal('show');
});


$(".date-ip").on("change", function () {
    this.setAttribute(
        "data-date",
        moment(this.value, "YYYY-MM-DD")
            .format(this.getAttribute("data-date-format"))
    );
}).trigger("change");


function getRecurssions() {
    let email = $("#contextemail").val();
    $.ajax({
        type: 'GET',
        url: '/api/GetRecurssions/' + email,
        success: function (data) {
            $(".loader").hide();
            $(".custom-tb").show();
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#questioncount").html("(" + recurssions.length + ")");
            daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
            let sendpostat = "";
            let blockdata = "";
            let wholedata = "";
            recurssions.forEach(x => {
                let timehours = "";
                let timeminutes = "";
                blockdata = "";
                let mode = ' AM';
                if (x.ExecutionTime) {
                    if ((new Date().getTimezoneOffset() / 60).toString().split('.').length > 1) {
                        timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                        timeminutes = parseInt(x.ExecutionTime.split(":")[1]) + parseInt((new Date().getTimezoneOffset() / 60).toString().split('.')[1] * 6);

                        if (timeminutes === 60) {
                            timehours = timehours + 1;
                            timeminutes = '00';
                        }

                        if (timehours > 11) {
                            mode = ' PM';
                        }
                        if (timehours > 12) {
                            timehours = timehours - 12;
                        }
                    }
                    else {
                        timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                        timeminutes = "00";
                        if (timehours > 11) {
                            mode = ' PM';
                        }
                        if (timehours > 12) {
                            timehours = timehours - 12;
                        }
                    }
                }
                if (x.RecurssionType === "Monthly") {
                    sendpostat = "Every Month " + new Date(x.ExecutionDate).getDate() + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Weekly") {
                    sendpostat = "Every Week " + weeks[new Date(x.ExecutionDate).getDay()] + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Every weekday") {
                    sendpostat = "Every Week Day " + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Custom") {
                    sendpostat = (new DOMParser).parseFromString(x.CustomRecurssionTypeValue, "text/html").
                        documentElement.textContent + " at " + timehours + ":" + timeminutes + mode;
                }
                blockdata = blockdata + '<tr id="row1"><td class="hw-r-u">' + x.Question + '<div class="hru-desc">Created by: ' + x.CreatedBy + ' on ' + new Date(x.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td><td class="edit-icon" id="edit' + x.RefID + '"></td><td class="delete-icon" id="deleteIcon' + x.RefID + '" data-toggle="modal" data-target="#myalert"></td></tr>';
                wholedata = wholedata + blockdata;

                $(document).on("click", "#deleteIcon" + x.RefID, function (event) {
                    $("#deleteIcon").show();
                    $("#managetable").hide();
                    deleteid = event.currentTarget.id.split('on')[1];
                    let ques = recurssions.find(x => x.RefID === deleteid);
                    $("#tabledeletebodydetails").html('<tr id="row1"><td class="hw-r-u">' + ques.Question + '<div class="hru-desc">Created by: ' + ques.CreatedBy + ' on ' + new Date(ques.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td></tr>');
                });

                $(document).on("click", "#edit" + x.RefID, function (event) {
                    $("#edit").show();
                    $("#managetable").hide();
                    $(".day-select,.eve-week-start,.month-cal").hide();
                    editid = event.currentTarget.id.split('it')[1];
                    let singledata = blockdata;
                    let ques = recurssions.find(x => x.RefID === editid);
                    $("#currentrecurrsionquestion").html(ques.Question);
                    let date = moment(ques.ExecutionDate).format("ddd MMM DD, YYYY");
                    $("#execdate").attr("data-date", date);
                    let timehours = "";
                    let timeminutes = "";
                    let mode = ' AM';

                    if (x.ExecutionTime) {
                        if ((new Date().getTimezoneOffset() / 60).toString().split('.').length > 1) {
                            timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                            timeminutes = parseInt(x.ExecutionTime.split(":")[1]) + parseInt((new Date().getTimezoneOffset() / 60).toString().split('.')[1] * 6);

                            if (timeminutes === 60) {
                                timehours = timehours + 1;
                                timeminutes = '00';
                            }

                            if (timehours > 11) {
                                mode = ' PM';
                            }
                            if (timehours > 12) {
                                timehours = timehours - 12;
                            }
                        }
                        else {
                            timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                            timeminutes = "00";
                            if (timehours > 11) {
                                mode = ' PM';
                            }
                            if (timehours > 12) {
                                timehours = timehours - 12;
                            }
                        }
                    }
                    if (x.RecurssionType === "Monthly") {
                        sendpostat = "Every Month " + new Date(x.ExecutionDate).getDate() + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $("#dwm").val("month");
                        $("#dwm").trigger("change");
                        $("#customnumber").html("1");
                        $("#customtype").html("month");
                    }
                    else if (x.RecurssionType === "Weekly") {
                        sendpostat = "Every Week " + weeks[new Date(x.ExecutionDate).getDay()] + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $("#dwm").val("week");
                        $("#dwm").trigger("change");
                        $("#" + weeks[new Date(x.ExecutionDate).getDay()]).addClass("selectedweek");
                        $("#customnumber").html("1");
                        $("#customtype").html("week");
                    }
                    else if (x.RecurssionType === "Every weekday") {
                        sendpostat = "Every Week Day " + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $("#dwm").val("week");
                        $("#dwm").trigger("change");
                        $(".weekselect").addClass("selectedweek");
                        $("#Sunday").removeClass("selectedweek");
                        $("#Saturday").removeClass("selectedweek");
                        $("#customtype").html("week day");
                    }
                    else if (x.RecurssionType === "Custom") {
                        sendpostat = (new DOMParser).parseFromString(x.CustomRecurssionTypeValue, "text/html").
                            documentElement.textContent + " at " + timehours + ":" + timeminutes + mode;
                        div = document.createElement('div');
                        $(div).html(x.CustomRecurssionTypeValue);
                        var type = $(div).find("#customtype").html().split('(')[0];
                        $("#dwm").val(type);
                        $("#dwm").trigger("change");
                        $("#number").val($(div).find("#customnumber").html());
                        if (type === 'month') {
                            data = sendpostat.split(' ');
                            if (data[0] === "Day") {
                                $("input[name='days-check']:checked").val("days");
                                $("#monthdate").val(data[1]);
                            }
                            else {
                                $("input[name='days-check']:checked").val("weeks");
                                $("#weekseries").val(data[0]);
                                $("#weekday").val(data[1]);
                            }
                        }
                        if (type === 'week') {
                            data = sendpostat.split(' ');
                            weekarray = data[3].split(',');
                            weekarray.forEach(week => {
                                $("#" + week).addClass('selectedweek');
                            });
                        }
                    }
                    $("#tablebodydetails").html('<tr id="row1"><td class="hw-r-u">' + ques.Question + '<div class="hru-desc">Created by: ' + ques.CreatedBy + ' on ' + new Date(ques.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td></tr>');

                });
            });
            $("#tablebody").html(wholedata);
            setTimeout(() => {
                recurssions.forEach(x => {
                    $(document).on("click", "#deleteIcon" + x.RefID, function (event) {
                        deleteid = event.currentTarget.id.split('on')[1];
                    });
                });
            }, 100);
            $("#deleteIcon").hide();
            $("#managetable").show();
        }
    });

}

function deleteRecurssion() {
    $.ajax({
        type: 'GET',
        url: '/api/DeleteReflection/' + deleteid,
        success: function (data) {
            if (data === "Deleted") {
                $("#tablebody").html("");
                getRecurssions();
            }

        }
    });
}

function saveRecurssion() {
    let rectype = "";
        if ($("#dwm").val() === "month") {
            if ($("input[name='days-check']:checked").val() === "days") {
                rectype = "Day " + $("#monthdate").val() + " " + $("#finaldates").html();
            }
            if ($("input[name='days-check']:checked").val() === "weeks") {
                rectype = $("#weekseries").val() + " " + $("#weekday").val() + " " + $("#finaldates").html();
            }

        }
        else rectype = $("#finaldates").html();
    $.ajax({
        type: 'POST',
        url: '/api/SaveRecurssionData',
        headers: {
            "Content-Type": "application/json"
        },
        data: JSON.stringify({
            "refID": editid, "recurssionType": "Custom", customRecurssionTypeValue: rectype
        }),
        success: function (data) {
            if (data === "true") {
                $("#tablebody").html("");
                $("#edit").hide();
                $("#managetable").show();
                getRecurssions();
            }
        }
    });
}

function gotoIndex() {
    let linkInfo = {
        action: "reflection"
    };
    microsoftTeams.tasks.submitTask(linkInfo);
    return true;
}

$("#dwm").on("change", function () {
    if (this.value === "day") {
        $(".eve-day-start").show();
        $(".card").removeClass("week month");
        $(".card").addClass("day");
        $(".day-select,.eve-week-start,.month-cal").hide();
        $("#slectedweeks").html();
    } else if (this.value === "week") {
        $(".day-select,.eve-week-start").show();
        $(".card").removeClass("day month");
        $(".card").addClass("week");
        $(".eve-day-start,.eve-month-start,.month-cal").hide();
        let slectedweeks = [];
        let weekdays = $(".weekselect");
        weekdays.each((x) => {
            if ($(weekdays[x]).hasClass("selectedweek")) {
                slectedweeks.push($(weekdays[x]).attr("id"));
            }
        });
        $("#slectedweeks").html(slectedweeks.join(","));
    } else {
        $(".day-select,.eve-week-start,.eve-day-start,.day-select").hide();
        $(".month-cal,.eve-month-start").show();
        $(".card").removeClass("week day");
        $(".card").addClass("month");
        $("#slectedweeks").html();
    }
    $("#startdatedisplay").html($("#execdate").val());
    $("#customnumber").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
});

$("#number").on("change", function () {
    $("#customnumber").html($("#number").val());
});

$("#monthdate").on("keyup", function () {
    if (this.value > 31) {
        this.value = this.value[0];
    }
});

$(".weekselect").on("click", function () {
    if ($(this).hasClass("selectedweek")) {
        $(this).removeClass("selectedweek");
    } else {
        $(this).addClass("selectedweek");
    }
    let slectedweeks = [];
    let weekdays = $(".weekselect");
    weekdays.each((x) => {
        if ($(weekdays[x]).hasClass("selectedweek")) {
            slectedweeks.push($(weekdays[x]).attr("id"));
        }
    });
    $("#slectedweeks").html(slectedweeks.join(","));
});

$("#recurrance").on("change", function () {
    if (this.value === "Custom") {
        $(".custom-cal").show();
        $(".day-select,.eve-week-start,.month-cal").hide();
    } else {
        $(".custom-cal").hide();
    }
});

function cancelRecurssion() {
    $("#edit").hide();
    $("#managetable").show();
}

function cancel() {
    $("#deleteIcon").hide();
    $("#managetable").show();
}