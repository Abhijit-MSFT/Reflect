let questions = [];
let userobject = {};
let accesstoken = "";

$(document).ready(function () {
    $(".js-example-basic-single").select2();
    $(".js-example-tags").select2({
        tags: true,
        maximumInputLength: 150
    });
    $("#privacytext").html($("#privacy").val());
    $("#usertext").html(" " + userName);
    let today = moment().format("YYYY-MM-DD");
    $("#execdate").val(today);
    $("#startdatedisplay").html(today);
    $("#customnumber").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
    $(".select2-selection__arrow").remove();
    let monthval = "";
    for (i = 1; i <= 31; i++) {
        monthval = monthval + '<option value="' + i + '">' + i + "</option>";
    }
    $("#monthdate").html(monthval);
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            let head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/Index.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/Index-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/Index-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
        GetDefaultQuestions(context.userPrincipalName);
    });

    $(".date-ip")
        .on("change", function () {
            this.setAttribute(
                "data-date",
                moment(this.value, "YYYY-MM-DD").format(
                    this.getAttribute("data-date-format")
                )
            );
            let today = moment().format("YYYY-MM-DD");
            if ($("#execdate").val() !== today) {
                $("#sendnow").attr("disabled", "true");
                $("#exectime").select2().val("00:00 AM").trigger("change");
                $(".select2-selection__arrow").remove();
            } else {
                $("#sendnow").removeAttr("disabled");
                $("#exectime").select2().val("Send now").trigger("change");
                $(".select2-selection__arrow").remove();
            }
            $("#startdatedisplay").html($("#execdate").val());
        })
        .trigger("change");
        $("#privacytext").html($("#privacy").val());
});

function SendAdaptiveCard() {
    let list = document.querySelectorAll(".htmlEle");
    let htmObj = {};
    list.forEach((obj, i) => {
        htmObj[obj.getAttribute("data-attr")] = obj.value;
    });
    let index = questions.findIndex((x) => x.question === $("#questions").val());
    let questionid = null;
    if (index !== -1) {
        questionid = questions[index].questionID;
        console.log(questionid);
    }
    let rectype = "";
    if ($("#recurrance").val() === "Custom") {
        if ($("#dwm").val() === "month") {
            if ($("input[name='days-check']:checked").val() === "days") {
                rectype = "Day " + $("#monthdate").val()+" "+$("#finaldates").html();
            }
            if ($("input[name='days-check']:checked").val() === "weeks") {
                rectype = $("#weekseries").val() + " " + $("#weekday").val()+" "+ $("#finaldates").html();
            }
            
        }
        else rectype = $("#finaldates").html();
    }
    else rectype = $("#recurrance").val();


    let exectime = "";
    if ($("#exectime").val() !== "Send now") {
        if ((new Date().getTimezoneOffset() / 60).toString().split('.').length > 1) {
            timehours = parseInt($("#exectime").val().split(":")[0]) - parseInt(-1 * new Date().getTimezoneOffset() / 60);
            timeminutes = parseInt($("#exectime").val().split(":")[1].split(' ')[0]) - parseInt((new Date().getTimezoneOffset() / 60).toString().split('.')[1] * 6);

            if (timeminutes === -30) {
                timehours = timehours - 1;
                timeminutes = '30';
            }

            if ($("#exectime").val().split(":")[1].split(' ')[1] === "PM") {
                timehours = timehours + 12;
            }
        }
        else {
            timehours = parseInt($("#exectime").val().split(":")[0]) - parseInt(-1 * new Date().getTimezoneOffset() / 60);
            timeminutes = "00";
            if ($("#exectime").val().split(":")[1].split('')[1] === "PM") {
                timehours = timehours + 12;
            }
        }
        exectime = timehours + ":" + timeminutes;

    }
    else
        exectime = $("#exectime").val();

    let taskInfo = {
        question: $("#questions").val(),
        questionID: questionid,
        privacy: $("#privacy").val(),
        executionDate: $("#execdate").val(),
        executionTime: exectime,
        nextExecutionDate: combineDateAndTime($("#execdate").val(), $("#exectime").val()),
        postDate: "",
        isDefaultQuestion: false,
        recurssionType: $("#recurrance").val(),
        customRecurssionTypeValue: rectype,
        action: "sendAdaptiveCard"
    };
    taskInfo.card = "";
    taskInfo.height = "medium";
    taskInfo.width = "medium";
    if (!$("#questions").val()) {
        alert("Please select " + $(".question").text());
    } else if (!$(".date-ip").val()) {
        alert("Please select " + $("#date").text());
    } else {
        microsoftTeams.tasks.submitTask(taskInfo);
    }
    return true;
}

function combineDateAndTime(date, time) {
    if ($('#exectime').val() !== "Send now") {
            time = getTwentyFourHourTime(time);
            return new Date(moment(`${date} ${time}`, 'YYYY-MM-DD HH:mm').format()).toUTCString();
        
    }
    else {
        return "";
    }

}

function getTwentyFourHourTime(time) {
    var hours = Number(time.match(/^(\d+)/)[1]);
    var minutes = Number(time.match(/:(\d+)/)[1]);
    var AMPM = time.match(/\s(.*)$/)[1].toLowerCase();

    if (AMPM === "pm" && hours < 12) hours = hours + 12;
    if (AMPM === "am" && hours === 12) hours = hours - 12;
    var sHours = hours.toString();
    var sMinutes = minutes.toString();
    if (hours < 10) sHours = "0" + sHours;
    if (minutes < 10) sMinutes = "0" + sMinutes;

    return sHours + ':' + sMinutes;
}

function getSelectedOption(event) {
    $("#selectedTxt").html($("#questions").val());
    if ($("#questions").val().length === 0) {
        $("#selectedTxt").text("No reflection question entered");
        $(".feeling").addClass("feeling-noquestion");
    } else {
        $(".feeling").removeClass("feeling-noquestion");
    }
}

function setPrivacy() {
    $("#privacytext").html($("#privacy").val());
}

function GetDefaultQuestions(userPrincipleName) {
    let blockdata = "";
    $.ajax({
        type: "GET",
        url: "api/GetAllDefaultQuestions/" + userPrincipleName,
        success: function (data) {
            questions = data;
            data.forEach((x) => {
                blockdata =
                    blockdata +
                    ' <option class="default-opt" id="' +
                    x.questionID +
                    '" value="' +
                    x.question +
                    '" title="' +
                    x.question +
                    '">' +
                    x.question +
                    "</option>";
            });
            $("#questions").append(blockdata);
            $("#selectedTxt").html($("#questions").val());
            $(".select2-search__field").attr("maxlength", "150");
            GetRecurssionsCount(userPrincipleName);
        }
    });
}
function GetRecurssionsCount(userPrincipleName) {
    $.ajax({
        type: "GET",
        url: "api/GetRecurssions/" + userPrincipleName,
        success: function (data) {
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#recurssionscount").html("(" + recurssions.length + ")");
        }
    });
}

submitHandler = (err, result) => {
    console.log("Reached submithandler!");
};

function openTaskModule() {
    let linkInfo = {
        action: "ManageRecurringPosts"
    };
    microsoftTeams.tasks.submitTask(linkInfo);
    return true;
}

function closeTaskModule() {
    let closeTaskInfo = {
        action: "closeFirstTaskModule"
    };
    microsoftTeams.tasks.submitTask(closeTaskInfo);
    return true;
}

function addShowHideButton() {
    if ($("#questionsblock").hasClass("hidequestions")) {
        $("#questionsblock").removeClass("hidequestions");
        $("#questionsblock").addClass("showquestions");
    } else {
        $("#questionsblock").addClass("hidequestions");
        $("#questionsblock").removeClass("showquestions");
    }
}

$("#recurrance").on("change", function () {
    if (this.value === "Custom") {
        $(".custom-cal").show();
        $(".day-select,.eve-week-start,.month-cal").hide();
    } else {
        $(".custom-cal").hide();
    }
});

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

