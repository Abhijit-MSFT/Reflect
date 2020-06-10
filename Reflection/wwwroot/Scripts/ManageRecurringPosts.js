var blockdata = ""
var deleteid = ""
var editid = ""
var weeks=["Sun","Mon","Tue","Wed","Thu","Fri","Sat"]
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
                    sendpostat = "Every Month " + new Date(x.RefCreatedDate).getDate() + " at " + x.ExecutionTime;
                }
                else if (x.RecurssionType == "Weekly"){
                    sendpostat = "Every Week " + weeks[new Date(x.RefCreatedDate).getDay()] + " at " + x.ExecutionTime;
                }
                else {
                    sendpostat = "Every Week Day " + " at " + new Date(x.RefCreatedDate).toLocaleTimeString('en-US', { hour: 'numeric', hour12: true, minute: 'numeric' });
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
                        editid = event.currentTarget.id.split('it')[1];
                        var ques = recurssions.find(x => x.RefID == editid)
                        $("#currentrecurrsionquestion").html(ques.Question)
                        $("#currentprivacy").val(ques.Privacy);
                        var time = new Date(ques.ExecutionDate).toLocaleDateString().split('/');
                        if (time.length) {
                            time.reverse();
                            if (time[1] < 10) {
                                time[1] = "0" + time[1]
                            }
                            if (time[2] < 10) {
                                time[2] = "0" + time[2]
                            }
                        }
                        $("#execdate").val(time.join('-'))
                        $("#exectime").val(ques.ExecutionTime)
                        $("#currentrecurrance").val(ques.RecurssionType);
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

function saveRecurssion() {
    $.ajax({
        type: 'POST',
        url: '/api/SaveRecurssionData',
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify({ "refID": editid, "executionTime": $("#exectime").val(), "executionDate": $("#execdate").val(), "privacy": $("#currentprivacy").val(), "recurssionType": $("#currentrecurrance").val() }),
        success: function (data) {
            if (data == "true") {
                $("#tablebody").html("");
                getRecurssions();
            }

        }
    });
}

