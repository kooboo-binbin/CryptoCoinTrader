var Message = function () {
    var self = this;
    self.start = function (vue) {
        var websocket = new WebSocket("ws://localhost:4888/message");
        websocket.onmessage = function (e) {
            //console.log(e.data);
            var o = JSON.parse(e.data);
            if (o.type == 0) {
                vue.$toastr.s(o.message);
            } else {
                vue.$toastr.e(o.message)
            }
        };
        websocket.onclose = function (e) {
           // console.log('ws message is closed');
            window.setTimeout(function () { self.start(vue); }, 3500);
        };
        websocket.onerror = function (e) {
           // console.log('ws message has an error');
            websocket.close();
        };
    };
};
var message = new Message();
export default message

