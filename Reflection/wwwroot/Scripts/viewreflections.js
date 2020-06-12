let contextPrincipalName;

$(document).ready(function () {
    $(".loader").show();
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme == "default") {
            var head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/openReflections.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme == "dark") {
            var head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/openReflections-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            var head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/openReflections-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
        contextPrincipalName = context.userPrincipalName;
    });
    GetReflections();
});

function GetReflections() {
    $.ajax({
        type: "GET",
        url: "/api/GetReflections/" + $("#reflectionid").val(),
        success: function (data) {
            $(".loader").hide();
            $(".modal-mb-sc2").show();
            $(".sc2br-blk").show();
            if (data) {
                data = JSON.parse(data);
            }
            if (data && data.feedback && data.reflection && data.question) {
                var feedback = JSON.parse(data.feedback);
                var reflection = JSON.parse(data.reflection);
                var question = JSON.parse(data.question);
                $("#createdby").text(reflection.CreatedBy);
                $("#questiontitle").text(question.Question);
                $("#privacy").text(reflection.Privacy);
                var blockdata = "";
                var color = "white";
                var totalcount = 0;
                var datacount = 0;
                var width = 0;
                var descriptio = "";
                var chatUrl = "https://teams.microsoft.com/l/chat/0/0?users=";
                Object.keys(JSON.parse(data.feedback)).forEach((x) => {
                    totalcount = totalcount + feedback[x].length;
                });
                for (i = 1; i <= 5; i++) {
                    if (Object.keys(feedback).indexOf(i.toString()) !== -1) {
                        datacount = feedback[i].length;
                        description =
                            reflection.Privacy == "anonymous"
                                ? ""
                                : feedback[i].map((x) => x.FullName).join(",");
                        width = ((datacount * 100) / totalcount).toFixed(0);
                    } else {
                        datacount = 0;
                        width = 0;
                        description = "";
                    }
                    if (i === 1) {
                        color = "green";
                        img = "Default_1.png";
                    } else if (i === 2) {
                        color = "light-green";
                        img = "Default_2.png";
                    } else if (i === 3) {
                        color = "orng";
                        img = "Default_3.png";
                    } else if (i === 4) {
                        color = "red";
                        img = "Default_4.png";
                    } else if (i === 5) {
                        color = "dark-red";
                        img = "Default_5.png";
                    }
                    blockdata =
                        blockdata +
                        '<div class="media"><img src="../Images/' +
                        img +
                        '" class="align-self-start smils" alt="smile2"><div class="media-body cb-smile2"><div class="progress custom-pr"><div class="progress-bar bg-' +
                        color +
                        '" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:' +
                        width.toString() +
                        '%"></div></div>';

                    if (description) {
                        feedback[i].forEach((data,index) => {
                            blockdata =
                                blockdata +
                                '<span class="smile-desc" id="' +
                                data.FeedbackID +
                                '">' +
                                data.FullName
                            blockdata = index+1 != feedback[i].length ? blockdata + ',' : blockdata +''
                            blockdata = blockdata+'</span><div class="card custom-profle-card ' +
                                data.FeedbackID +
                                '"> <div class="card-body"> <img src="../Images/Avatar.png" alt="avatar" class="profile-pic" /> <div class="profile-name">' +
                                data.FullName +
                                '</div > <div class="start-chat" style = "pointer-events: ' + GetChatConfig(data.FeedbackGivenBy) + ';"  > <span class="chat-icon" onclick = "microsoftTeams.executeDeepLink(' + "'" + chatUrl + data.FeedbackGivenBy + "'" + ');" ></span > <span class="st-chat-txt">Start a chat</span> </div > <div class="mail"> <span class="mail-icon"></span> <span class="mail-txt"> ' +
                                data.FeedbackGivenBy +
                                " </span> </div> </div > </div > ";
                        });
                    }
                    //blockdata = feedback[i].length > 5 ? blockdata + 'more' : blockdata;
                    //enable this for detailed screen
                    //blockdata = blockdata + '<span onclick=openDetailReflection(' + i + ',"'+reflection.ReflectionID+'")> more</span>'
                    blockdata =
                        blockdata +
                        '</div><div class="cnt-box">' +
                        width +
                        '%<span class="cnt">(' +
                        datacount +
                        ")</span></div ></div >";
                }
                $("#reviewblock").html(blockdata);
                $(".custom-profle-card ").css("display", "none");
                $(".smile-desc").hover(function (event) {
                    $(".custom-profle-card").css("display", "none");
                    $("." + $(event.target)[0].id).show();
                });

                $("body").on("click blur", function (event) {
                    if (!$(event.target).hasClass("custom-profle-card")) {
                        $(".custom-profle-card").css("display", "none");
                    }
                });

                $(".custom-profle-card > *").on("click", function (e) {
                    e.stopPropagation();
                });
            } else {
                alert("no data");
            }
        },
    });
}
function GetChatConfig(userId) {
    return (userId == contextPrincipalName) ? "none" : "all";
};

function openDetailReflection(feedback,reflectionid) {
    let linkInfo = {
        action: "OpenDetailfeedback",
        feedback: feedback,
        reflectionID: reflectionid
    };
    microsoftTeams.tasks.submitTask(linkInfo);
    return true;
}