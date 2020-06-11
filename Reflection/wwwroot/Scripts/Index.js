var questions = [];
var userobject = {};
var accesstoken = "";


$(document).ready(function () {
    $('.js-example-basic-single').select2();
    $(".js-example-tags").select2({
        tags: true
    });
    $("#usertext").html(" " + userName);
    var today = moment().format('YYYY-MM-DD');
    $('#execdate').val(today);
    $('.select2-selection__arrow').remove()
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            var head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/Index.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            var head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/Index-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            var head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/Index-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
        GetDefaultQuestions(context.userPrincipalName);
    });
    $("body").click(function (e) {
        if (e.target.className !== "form-control questioninput") {
            $("#questionsblock").addClass("hidequestions");
            $("#questionsblock").removeClass("showquestions");
        }
    });
    //$("select")
    //    .change(function () {
    //        $(this)
    //            .find("option:selected")
    //            .each(function () {
    //                var optionValue = $(this).attr("value");
    //                if (optionValue) {
    //                    $(".box")
    //                        .not("." + optionValue)
    //                        .hide();
    //                    $("." + optionValue).show();
    //                } else {
    //                    $(".box").hide();
    //                }
    //            });
    //        if ($('#questions-list').val().length === 0) {
    //            $('#selectedTxt').text("No reflection question entered");
    //            $('.feeling').addClass("feeling-noquestion");
    //        } else {
    //            $('.feeling').removeClass("feeling-noquestion");
    //        }
    //    })
    //    .change();

    $(".date-ip").on("change", function () {
        
        this.setAttribute(
            "data-date",
            moment(this.value, "YYYY-MM-DD")
                .format(this.getAttribute("data-date-format"))
        )
        var today = moment().format('YYYY-MM-DD');
        if ($('#execdate').val() !== today) {
            $('#sendnow').attr("disabled", "true");
            $('#exectime').select2().val("00:00 AM").trigger("change")
        }
        else {
            $('#sendnow').removeAttr("disabled");
            $('#exectime').select2().val("Send now").trigger("change")
        }
    }).trigger("change")
});


function SendAdaptiveCard() {
    var list = document.querySelectorAll(".htmlEle");
    var htmObj = {};
    list.forEach((obj, i) => {
        htmObj[obj.getAttribute("data-attr")] = obj.value;
    });
    let index = questions.findIndex(
        (x) => x.question === $("#questions").val()
    );
    var questionid = null;
    if (index !== -1) {
        questionid = questions[index].questionID;
        console.log(questionid);
    }

    let taskInfo = {
        question: $("#questions").val(),
        questionID: questionid,
        privacy: $("#privacy").val(),
        executionDate: $("#execdate").val(),
        executionTime: $("#exectime").val(),
        postDate: "",
        isDefaultQuestion: false,
        //postSendNowFlag: true,
        recurssionType: $("#recurrance").val(),
        action: "sendAdaptiveCard",
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

    function getSelectedOption(event) {
        $('#selectedTxt').html($("#questions").val());
        if ($('#questions').val().length === 0) {
            $('#selectedTxt').text("No reflection question entered");
            $('.feeling').addClass("feeling-noquestion");
        } else {
            $('.feeling').removeClass("feeling-noquestion");
        }
}

$('#timepick').timepicker({
    timeFormat: 'h:mm p',
    interval: 30,
    minTime: '12:00 am',
    maxTime: '11:30pm',
    defaultTime: 'auto',
    dynamic: false,
    dropdown: true,
    scrollbar: false
});

$('#timepick').timepicker('setTime', new Date().getHours() + ':' + new Date().getMinutes());

function setPrivacy() {
    $("#privacytext").html($("#privacy").val());
}

function GetDefaultQuestions(userPrincipleName) {
    var blockdata = "";
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
                    '">'+x.question+'</option>';
               
            });
            $("#questions").html(blockdata);
            $("#selectedTxt").html($("#questions").val());
            GetRecurssionsCount(userPrincipleName);
        },
    });
}
function GetRecurssionsCount(userPrincipleName) {
    $.ajax({
        type: "GET",
        url: "api/GetRecurssions/" + userPrincipleName,
        success: function (data) {
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#recurssionscount").html("(" + recurssions.length + ")");
        },
    });
}


submitHandler = (err, result) => {
    //if (result.action == "Chaining") {
    //        let taskInfo = {
    //        title: "",
    //        height: "",
    //        width: "",
    //        Url: ""
    //    };
    //    taskInfo.Url = "https://1a48ca6e.ngrok.io/First";
    //    taskInfo.height = 550;
    //    taskInfo.width = 780;
    //    microsoftTeams.tasks.startTask(taskInfo, submitHandler)
    //}
    console.log("Reached submithandler!");
};

function openTaskModule() {
    let linkInfo = {
        action: "ManageRecurringPosts",
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

$('#recurrance').on('change', function () {
    if (this.value == 'Custom') {
        $(".custom-cal").show();
    }
    else {
        $(".custom-cal").hide();
    }

});

$('#dwm').on('change', function () {
    if (this.value == 'day') {
        $(".eve-day-start").show();
        $(".card").removeClass("week month");
        $(".card").addClass("day");
        $(".day-select,.eve-week-start,.month-cal").hide();
    }
    else if (this.value == 'week') {
        $(".day-select,.eve-week-start").show();
        $(".card").removeClass("day month");
        $(".card").addClass("week");
        $(".eve-day-start,.eve-month-start,.month-cal").hide();
    }
    else {
        $(".day-select,.eve-week-start,.eve-day-start,.day-select").hide();
        $(".month-cal,.eve-month-start").show();
        $(".card").removeClass("week day");
        $(".card").addClass("month");
    }
});