let accesstoken;
let contextPrincipalName;
$(document).ready(function () {
    $(".loader").show();
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        contextPrincipalName = context.userPrincipalName;
    });
    $.ajax({
        url: "/api/GetAccessTokenAsync",
        type: "Get",
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'Accept': 'application/json'
        },
        success: function (token) {
            accesstoken = token;
        },
        complete: function () {
            if (accesstoken !== undefined && accesstoken !== null && accesstoken !== '') {
                GetReflections();
            }
        }
    });
});

function GetReflections() {
    $.ajax({
        type: 'GET',
        url: '/api/GetReflections/' + $("#reflectionid").val(),
        success: function (data) {
            $(".loader").hide();
            $(".modal-mb-sc2").show();
            if (data) {
                data = JSON.parse(data);
            }
            if (data && data.feedback && data.reflection && data.question) {
                var feedback = JSON.parse(data.feedback);
                var reflection = JSON.parse(data.reflection);
                var question = JSON.parse(data.question)
                $("#createdby").text(reflection.CreatedBy);
                $("#questiontitle").text(question.Question);
                $("#privacy").text(reflection.Privacy);
                var blockdata = ''
                var color = "white";
                var totalcount = 0;
                var datacount = 0;
                var width = 0;
                var descriptio = "";
                var chatUrl = "https://teams.microsoft.com/l/chat/0/0?users=";
                Object.keys(JSON.parse(data.feedback)).forEach(x => {
                    totalcount = totalcount + feedback[x].length;
                })
                for (i = 1; i <= 5; i++) {
                    if (Object.keys(feedback).indexOf(i.toString()) !== -1) {
                        datacount = feedback[i].length;
                        description = reflection.Privacy == "anonymous" ? "" : feedback[i].map(x => x.FullName).join(',');
                        width = ((datacount * 100) / totalcount).toFixed(0);

                    }
                    else {
                        datacount = 0;
                        width = 0;
                        description = "";
                    }
                    if (i == 1) {
                        color = "green";
                        img = "1.png";
                    }
                    else if (i == 2) {
                        color = "light-green";
                        img = "2.png";
                    }
                    else if (i == 3) {
                        color = "orng";
                        img = "3.png";
                    }
                    else if (i == 4) {
                        color = "red";
                        img = "4.png";
                    }
                    else if (i == 5) {
                        color = "dark-red";
                        img = "5.png";
                    }
                    blockdata = blockdata + '<div class="media"><img src="../Images/' + img + '" class="align-self-start smils" alt="smile2"><div class="media-body cb-smile2"><div class="progress custom-pr"><div class="progress-bar bg-' + color + '" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:' + width.toString() + '%"></div></div>';

                    if (description) {
                        feedback[i].forEach(data => {
                            blockdata = blockdata + '<div class="smile-desc" id="' + data.FeedbackID + '">' + data.FullName + '</div><div class="card custom-profle-card ' + data.FeedbackID + '"> <div class="card-body"> <img src="' + GetPhoto(data.FeedbackGivenBy, accesstoken) + '" alt="avatar" class="profile-pic" /> <div class="profile-name">' + data.FullName + '</div > <div class="start-chat" style = "pointer-events: ' + GetChatConfig(data.FeedbackGivenBy) + ';"  > <span class="chat-icon" onclick = "microsoftTeams.executeDeepLink(' + "'" + chatUrl + data.FeedbackGivenBy + "'" + ');" ></span > <span class="st-chat-txt">Start a chat</span> </div > <div class="mail"> <span class="mail-icon"></span> <span class="mail-txt"> ' + data.FeedbackGivenBy + ' </span> </div> </div > </div > '
                        })
                    }
                    blockdata = blockdata + '</div><div class="cnt-box">' + width + '%<span class="cnt">(' + datacount + ')</span></div ></div >';
                }
                $("#reviewblock").html(blockdata);
                $(".custom-profle-card ").css("display", "none");
                $('.smile-desc').hover(function (event) {
                    $(".custom-profle-card").css("display", "none");
                    $("." + $(event.target)[0].id).show();
                });

                $('body').on('click blur', function (event) {
                    if (!$(event.target).hasClass('custom-profle-card')) {
                        $(".custom-profle-card").css("display", "none");
                    }
                });

                $('.custom-profle-card > *').on('click', function (e) {
                    e.stopPropagation();
                });

            }
            else {
                alert("no data");
            }
        }
    });
}

function GetChatConfig(userId) {
    return (userId == contextPrincipalName) ? "none" : "all";
};
function GetPhoto(userid, accesstoken) {
    var profilepath = "../Images/Avatar.png";
    $.ajax({
        url: "/ProfilePhoto",
        data: { 'token': accesstoken, 'userid': userid },
        type: "GET",
        async: false,
        success: function (response) {
            if (response !== null && response != "") {
                profilepath = response;
            } else {
                alert("Something went wrong");
            }
        },
        error: function () {
            console.log("Failed");
        }
    });
    return profilepath;
}