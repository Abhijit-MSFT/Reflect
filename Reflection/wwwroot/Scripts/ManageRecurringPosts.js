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
    var blockdata = ""
    $.ajax({
        type: 'GET',
        url: 'api/GetRecurssions',
        success: function (data) {
            $(".loader").hide();
            $(".custom-tb").show();
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#questioncount").html("(" + recurssions.length + ")");
            daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
            var sendpostat = "";
            recurssions.forEach(x => {
                if (x.RecurssionType == "Monthly") {
                    sendpostat = "Every Month " + new Date(x.RefCreatedDate).getDate() + " at " + new Date(x.RefCreatedDate).toLocaleTimeString('en-US', { hour: 'numeric', hour12: true, minute: 'numeric' });
                }
                else if (x.RecurssionType == "Weekly") {
                    sendpostat = "Every Week " + daysInWeek[new Date(x.RefCreatedDate).getDay()] + " at " + new Date(x.RefCreatedDate).toLocaleTimeString('en-US', { hour: 'numeric', hour12: true, minute: 'numeric' });
                }
                else {
                    sendpostat = "Every Week Day"
                }
                blockdata = blockdata + '<tr id="row1"><td class="hw-r-u">' + x.Question + '<div class="hru-desc">Created by: ' + x.CreatedBy + ' on ' + new Date(x.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td><td class="delete-icon" data-toggle="modal" data-target="#myalert"></td></tr>';
            })
            $("#tablebody").html(blockdata);

        }
    });
});

//$(document).on("click", ".delete-icon", function () {
//    if (confirm('Are you sure to delete ?')) {
//        $(this).prev('#').remove();
//    }
//});

$('.delete-icon').click(function () {
    $('#myModal').modal('show');
});