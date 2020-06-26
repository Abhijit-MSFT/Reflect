let blockdata = ""
let deleteid = ""
let editid = ""
let weeks=["Sun","Mon","Tue","Wed","Thu","Fri","Sat"]
$(document).ready(function () {
    $(".loader").show();
    let today = moment().format("YYYY-MM-DD");
    $("#execdate").val(today);
    $("#startdatedisplay").html(today);
    $("#customnumber").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
    $('[data-toggle="popover"]').popover()
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
    $('#myModal').modal('show');
});

$('.edit-icon').click(function () {
    $('#edit').modal('show');
});


$(".date-ip").on("change", function () {
    this.setAttribute(
        "data-date",
        moment(this.value, "YYYY-MM-DD")
            .format(this.getAttribute("data-date-format"))
    )
}).trigger("change")


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
            blockdata = "";
            recurssions.forEach(x => {
                let timehours=""
                let timeminutes = ""
                let mode = ' AM'
                if (x.ExecutionTime) {
                     timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt((-1 * new Date().getTimezoneOffset()) / 60)
                     timeminutes = parseInt(x.ExecutionTime.split(":")[1]) + Math.floor((-1 * new Date().getTimezoneOffset()) / 60) * 6
                    
                    if (timeminutes === 60) {
                        timehours = timehours + 1
                        timeminutes = '00';
                    }

                    if (timehours > 11) {
                        mode = ' PM'
                    }
                    if (timehours > 12) {
                        timehours = timehours - 12
                    }
                }
                if (x.RecurssionType === "Monthly") {
                    sendpostat = "Every Month " + new Date(x.ExecutionDate).getDate() + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Weekly"){
                    sendpostat = "Every Week " + weeks[new Date(x.ExecutionDate).getDay()] + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType ==="Every weekday"){
                    sendpostat = "Every Week Day " + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else 
                {
                    sendpostat = x.RecurssionType + " at " + timehours + ":" + timeminutes + mode;
                }
                blockdata = blockdata + '<tr id="row1"><td class="hw-r-u">' + x.Question + '<div class="hru-desc">Created by: ' + x.CreatedBy + ' on ' + new Date(x.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td><td class="edit-icon" id="edit' + x.RefID + '" data-toggle="modal" data-target="#edit"></td><td class="delete-icon" id="delete' + x.RefID + '" data-toggle="modal" data-target="#myalert"></td></tr>';
            })
            $("#tablebody").html(blockdata);
            setTimeout(() => {
                recurssions.forEach(x => {
                    $(document).on("click", "#delete" + x.RefID, function (event) {
                        deleteid = event.currentTarget.id.split('te')[1];
                    });
                });
            }, 100);
            setTimeout(() => {
                recurssions.forEach(x => {
                    $(document).on("click", "#edit" + x.RefID, function (event) {
                        $(".day-select,.eve-week-start,.month-cal").hide();
                        editid = event.currentTarget.id.split('it')[1];
                        let ques = recurssions.find(x => x.RefID === editid);
                        $("#currentrecurrsionquestion").html(ques.Question);
                        let date = moment(ques.ExecutionDate).format("ddd MMM DD, YYYY");
                        $("#execdate").attr("data-date", date)
                        let timehours = "";
                        let timeminutes = "";
                        let mode = ' AM';
                        if (x.ExecutionTime) {
                             timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt((-1 * new Date().getTimezoneOffset()) / 60)
                             timeminutes = parseInt(x.ExecutionTime.split(":")[1]) + Math.floor((-1 * new Date().getTimezoneOffset()) / 60) * 6
                            
                            if (timeminutes === 60) {
                                timehours = timehours + 1
                                timeminutes = '00';
                            }

                            if (timehours > 11) {
                                mode = ' PM'
                            }
                            if (timehours > 12) {
                                timehours = timehours - 12
                            }
                        }
                        $("#exectime").val(timehours + ":" + timeminutes + mode);
                        if (ques.RecurssionType !== "Weekly" && ques.RecurssionType !== "Monthly" && ques.RecurssionType !== "Every weekday") {
                            $("#recurrance").val("Custom");
                            $(".custom-cal").show();
                            $("#finaldates").html(ques.RecurssionType);
                            $("#number").val($("#customnumber").html());
                        }
                        else {
                            $(".custom-cal").hide();
                            $("#recurrance").val(ques.RecurssionType);
                        }
                        
                    });
                });
            }, 100);

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
    $.ajax({
        type: 'POST',
        url: '/api/SaveRecurssionData',
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify({ "refID": editid, "executionTime": $("#exectime").val(), "executionDate": $("#execdate").val(), "privacy": $("#currentprivacy").val(), "recurssionType": $("#currentrecurrance").val() }),
        success: function (data) {
            if (data === "true") {
                $("#tablebody").html("");
                getRecurssions();
            }

        }
    });
}

function gotoIndex() {
    let linkInfo = {
        action: "reflection",
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