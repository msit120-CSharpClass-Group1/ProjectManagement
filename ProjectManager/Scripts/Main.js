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
    $('.dropup-content').children().click(function () {
        $(this).parent().fadeToggle();
    });

    //強調左邊欄選單
    var leftnavli = $("#myLeftsidenav > ul > li");
    leftnavli.click(function () {
        leftnavli.removeClass('active');
        $(this).addClass('active');
    });
    //登出按鈕
    $('#logoutbtn').click(function () {
        window.location.href = '/Login/Logout';
    });
    //通知訊息下拉選單
    window.setInterval(msgcount, 5000);
    function msgcount()
    {
        $.post('/Notification/Load/', {}, function (datas) {
            var btn = $('#notificationbtn');
            btn.html('');
            var Fragdoc = $(document.createDocumentFragment());
            var _count = $(datas).length;
            var count = $('<span id="msgcount" class="label"></span>');
            if (_count >= 5) {
                count.html(_count+"+");
            }
            else
            {
                count.html(_count);
            }
            
            var i = $('<i class="far fa-envelope"></i>');
            Fragdoc.append(i); 
            if (_count !== 0) {
                Fragdoc.append(count);
            }
            btn.html(Fragdoc);
        });
    }
    msgcount();
    $('#notificationbtn').click(function () {
        var msg = $('#notificationmsg');
        if (msg.css('display') === 'block') {
            $.post('/Notification/Load/', {}, function (datas) {
                var Fragdoc = $(document.createDocumentFragment());
                $(datas).each(function (id, data) {
                    console.log(data.TaskName+','+data.IsRead);
                    var time = new Date(data.AssignedDate.match(/\d+/)[0] * 1);
                    var taskName = $('<h5></h5>').html('新增工作「' + data.TaskName + '」');
                    var taskTime = $('<h6></h6>').html(time.toLocaleDateString());
                    var task = $('<a></a>');
                    task.append(taskName);
                    task.append(taskTime);
                    Fragdoc.append(task);
                });
                var moreTask = $('<a></a>').append('更多訊息');
                Fragdoc.append(moreTask);
                msg.html(Fragdoc);
            });
        }
    });

});