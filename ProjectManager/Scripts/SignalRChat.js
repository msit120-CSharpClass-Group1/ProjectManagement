
//聊天室
$(document).ready(function () {
    var $test = $('#test');
    chatHub = $.connection.chatHub;

    chatHub.client.gotMessage = function (nickname, message) {
        //$test.append('<div><span>' + htmlEncode(nickname) + ":     " + '</span>' + htmlEncode(message) + '</div>');
        var avatar = $('<span class="msg-avatar">' + htmlEncode(nickname) + ':' + '</span>');
        var msgText = $('<div class="cm-msg-text">' + htmlEncode(message) + '</div>');
        var chatMsg = $('<div class="chat-msg self"></div>').append(avatar, msgText);
        $test.append(chatMsg);
    };

    var htmlEncode = function (content) {        //怕XS攻擊?//做一個小保護
        return $('<div />').text(content).html();
    }

    $.connection.hub.start().done(function () {
        $("#sendmessage").click(function (evt) {
            var nickname = $("#wantname").text(),
                $message = $("#message"),
                message = $message.val();

            chatHub.server.sendMessage(nickname, message);

            $message.val("").focus();
        });
    });
});