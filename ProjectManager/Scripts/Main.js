$(document).ready(function () {
    //開啟左側選單
    $('.leftOpenbtn').click(function () {
        $('#myLeftsidenav').toggleClass("left-sidenav-open");
        $('#myLeftsidenav').toggleClass("left-sidenav-collapse");
        $('.main').toggleClass("main-left-toggle");
    });

    //開啟右側選單
    $('.rightOpenbtn').click(function ()
    {
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

});