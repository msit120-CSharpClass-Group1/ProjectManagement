$(document).ready(function () {
    //開啟左側選單
    $('.leftOpenbtn').click(function () {
        $('#myLeftsidenav').toggleClass("left-sidenav-open");
        $('#myLeftsidenav').toggleClass("left-sidenav-collapse");
        $('.main').toggleClass("main-left-toggle");
    });

    //開啟右側選單
    $('.rightOpenbtn').click(function () {
        //$('.main').toggleClass("right-sidenav-toggle");
        $('#myRightsidenav').slideToggle();
    });

    //下拉式選單顯示/隱藏
    $(".dropdown-btn").click(function () {
        $(this.nextElementSibling).slideToggle();
    });

    //顯示/隱藏通知列表
    $('.navdropbtn').click(function () {
        $(this).next().fadeToggle();
    });

    //點及通知內容後隱藏通知列表
    $('.dropdown-content').children().click(function () {
        $(this).parent().fadeToggle();
    });

    //顯示/隱藏訊息列表
    $('.dropupbtn').click(function () {
        $('.dropup-content').fadeToggle();
    });

    //點及訊息內容後隱藏訊息列表
    //$('.dropup-content').children().click(function () {
    //    $(this).parent().fadeToggle();
    //});

    //強調左邊欄選單
    //var leftnavli = $("#myLeftsidenav > ul > li");
    //leftnavli.click(function () {
    //    leftnavli.removeClass('active');
    //    $(this).addClass('active');
    //});
    //登出按鈕
    $('#logoutbtn').click(function () {
        window.location.href = '/Login/Logout';
    });
    //通知訊息下拉選單
    window.setInterval(msgcount, 120000);
    msgcount();
    function msgcount() {
        $.post('/Notification/Load/', {}, function (datas) {
            var btn = $('#notificationbtn');
            btn.html('');
            var Fragcount = $(document.createDocumentFragment());
            var _count = 0;
            var count = $('<span id="msgcount" class="label"></span>');
            var i = $('<i class="far fa-envelope"></i>');
            Fragcount.append(i);
            var Fragdoc = $(document.createDocumentFragment());
            var msg = $('#notificationmsg');
            $(datas).each(function (id, data) {

                var time = new Date(data.NotificationDate.match(/\d+/)[0] * 1);
                var taskName;
                var taskTime = $('<span></span>').html(time.toLocaleDateString());
                var task = $('<a></a>');
                task.click(function () {
                    $.post("/Home/SetCookiesForMyBoard/", { projectGUID: data.ProjectGUID },
                        function () {
                            location.href = "/MyBoard/Index/" + data.EmployeeGUID;
                        });
                });
                if (data.IsRead !== true) {
                    task.addClass('IsRead');
                    _count++;
                }
                switch (data.Category) {
                    case 'Task':
                        taskName = $('<p></p>').html('「' + data.MangerName + '」在「' + data.ProjectName + '」分配了「' + data.TaskName + '」給你');
                        break;
                    case 'InvideProject':
                        taskName = $('<p></p>').html('「' + data.MangerName + '」邀請你加入了「' + data.ProjectName + '」專案');
                        break;
                }
                task.append(taskName);
                task.append(taskTime);
                Fragdoc.append(task);

            });
            var moreTask = $('<a></a>').append('更多訊息');
            moreTask.attr("href", "/Notification/Index/");
            Fragdoc.append(moreTask);
            msg.html(Fragdoc);
            if (_count >= 5) {
                count.html(_count + "+");
            }
            else {
                count.html(_count);
            }
            if (_count !== 0) {
                Fragcount.append(count);
            }
            btn.html(Fragcount);
        });
    }
});