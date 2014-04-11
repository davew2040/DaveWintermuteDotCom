

var LinkDisplayer = function () {
    var _$dialogDiv = null;
    var _$contentDiv = null;
    var _initialized = false;

    function setValue(text) {
        _$contentDiv.find("input").val(text);
        _$contentDiv.find("input")[0].select();
    }

    var self = {
        showLink: function (link) {
            if (!_initialized) {
                initialize();
                _initialized = true;
            }

            setValue(link);
            _$dialogDiv.dialog("open");
        },
        destroy: function () {
            _$dialogDiv.destroy();
        }
    };

    function initialize() {
        _$contentDiv = $("<div><input type='text'></input></div>");
        _$contentDiv.addClass("linkDisplayerContent");

        _$dialogDiv = $("<div></div>");
        _$dialogDiv.addClass("linkDisplayer");
        _$contentDiv.appendTo(_$dialogDiv);

        $("html").append(_$dialogDiv);
        _$dialogDiv.dialog({
            title: "Image Link Copier",
            width: 500,
            height: 100,
            resizable: false
        }, "close");

        _initialized = true;
    }

    return self;
}();