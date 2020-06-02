var questions = [];
var userobject = {};
var accesstoken = "";
$(document).ready(function () {
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme == "default") {
            var head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/Index.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme == "dark") {
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
    });
    $.ajax({
        url: "/api/GetAccessTokenAsync",
        type: "Get",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            Accept: "application/json",
        },
        success: function (token) {
            accesstoken = token;
            microsoftTeams.getContext(function (context) {
                if (token !== undefined && token !== null && token !== "") {
                    GetUserDetails(context.userPrincipalName, token);
                    GetDefaultQuestions(context.userPrincipalName);
                }
            });
        },
    });
    $("body").click(function (e) {
        if (e.target.className !== "form-control questioninput") {
            $("#questionsblock").addClass("hidequestions");
            $("#questionsblock").removeClass("showquestions");
        }
    });
    $("select")
        .change(function () {
            $(this)
                .find("option:selected")
                .each(function () {
                    var optionValue = $(this).attr("value");
                    if (optionValue) {
                        $(".box")
                            .not("." + optionValue)
                            .hide();
                        $("." + optionValue).show();
                    } else {
                        $(".box").hide();
                    }
                });
        })
        .change();

    $(".date-ip").on("change", function () {
        this.setAttribute(
            "data-date",
            moment(this.value, "YYYY-MM-DD")
                .format(this.getAttribute("data-date-format"))
        )
    }).trigger("change")

});

$('#questions-list').keyup(function () {
    if ($(this).val().length == 0) {
        $('.btn-send').hide();
    } else {
        $('.btn-send').show();
    }
}).keyup(); 

function SendAdaptiveCard() {
    var list = document.querySelectorAll(".htmlEle");
    var htmObj = {};
    list.forEach((obj, i) => {
        htmObj[obj.getAttribute("data-attr")] = obj.value;
    });
    let index = questions.findIndex(
        (x) => x.question === $("#questions-list").val()
    );
    var questionid = null;
    if (index !== -1) {
        questionid = questions[index].questionID;
        console.log(questionid);
    }

    let taskInfo = {
        question: $("#questions-list").val(),
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
    if (!$("#questions-list").val()) {
        alert("Please select " + $(".question").text());
    } else if (!$(".date-ip").val()) {
        alert("Please select " + $("#date").text());
    } else {
        microsoftTeams.tasks.submitTask(taskInfo);
    }
    return true;
}

function getSelectedOption(event) {
    $("#selectedTxt").html($("#questions-list").val());
}

function setPrivacy() {
    microsoftTeams.getContext(function (context) {
        if (
            accesstoken !== undefined &&
            accesstoken !== null &&
            accesstoken !== ""
        ) {
            GetUserDetails(context.userPrincipalName, accesstoken);
        }
    });
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
                    '">';
            });
            $("#questions").html(blockdata);
        },
    });
}

function GetUserDetails(principalName, appAccessToken) {
    $.ajax({
        url: "https://graph.microsoft.com/beta/users/" + principalName,
        type: "GET",
        beforeSend: function (request) {
            request.setRequestHeader("Authorization", "Bearer " + appAccessToken);
        },
        success: function (response) {
            console.log("Success");
            if (response !== null) {
                var name = response.displayName;
                alert(userNameArray[0]);
                var userNameArray = name.split(" ");
                console.log(userNameArray[0]);
                alert(userNameArray[0]);
                $("#usertext").html(" " + userNameArray[0] + " " + userNameArray[1]);
            } else {
                alert("Something went wrong");
            }
        },
        error: function () {
            console.log("Failed");
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
        action: "closeFirstTaskModule",
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

