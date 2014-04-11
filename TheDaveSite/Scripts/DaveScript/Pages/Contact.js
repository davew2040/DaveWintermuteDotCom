$(document).ready(function(){
    ContactScript.centerContent();
});

var ContactScript = new function () {
    var self = {
        centerContent: function () {
            var header = $("header");
            var footer = $("footer");

            $(".emailDivContainer").css("top", header.height());
            $(".emailDivContainer").css("bottom", footer.height());
        }
    };

    return self;
}();