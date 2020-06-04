﻿var blockdata = ""
var deleteid = ""
$(document).ready(function () {
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme == "default") {
            var head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/ManageRecurringPosts.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme == "dark") {
            var head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/ManageRecurringPosts-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            var head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/ManageRecurringPosts-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
    });
    $(".loader").show();
    getRecurssions();
});



$('.delete-icon').click(function () {
    $('#myModal').modal('show');
});


function getRecurssions() {
    var email = $("#contextemail").val();
    $.ajax({
        type: 'GET',
        url: '/api/GetRecurssions/' + email,
        success: function (data) {
            $(".loader").hide();
            $(".custom-tb").show();
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#questioncount").html("(" + recurssions.length + ")");
            daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
            var sendpostat = "";
            blockdata = "";
            recurssions.forEach(x => {
                if (x.RecurssionType == "Monthly") {
                    sendpostat = "Every Month " + new Date(x.RefCreatedDate).getDate() + " at " + new Date(x.RefCreatedDate).toLocaleTimeString('en-US', { hour: 'numeric', hour12: true, minute: 'numeric' });
                }
                else {
                    sendpostat = "Every Week Day"
                }
                blockdata = blockdata + '<tr id="row1"><td class="hw-r-u">' + x.Question + '<div class="hru-desc">Created by: ' + x.CreatedBy + ' on ' + new Date(x.RefCreatedDate).toDateString() + '</div></td><td>' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td><td class="edit-icon" id="edit' + x.RefID + '"></td><td class="delete-icon" id="delete' + x.RefID + '" data-toggle="modal" data-target="#myalert"></td></tr>';
            })
            $("#tablebody").html(blockdata);
            setTimeout(() => {
                recurssions.forEach(x => {
                    $(document).on("click", "#delete" + x.RefID, function (event) {
                        deleteid = event.currentTarget.id.split('te')[1];
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
            if (data == "Deleted") {
                $("#tablebody").html("");
                getRecurssions();
            }

        }
    });
}