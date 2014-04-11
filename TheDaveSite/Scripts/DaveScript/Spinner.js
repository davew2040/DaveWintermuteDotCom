var Spinner = function (targetDiv) {
    var activeCount = 0;

    function hide() {
        $targetDiv.css("display", "none");
    }

    var self = {
        show: function () {
            if (activeCount > 0) {
                activeCount++;
                return;
            }

            activeCount++;
            var $parent = $($targetDiv.parent());

            $targetDiv.css("left", $(window).width() / 2.0 - $targetDiv.width() / 2.0);
            $targetDiv.css("top", $(window).height() / 2.0 - $targetDiv.height() / 2.0);
            $targetDiv.css("display", "inline");
        },
        hide: function () {
            if (activeCount === 0) {
                throw "Can't hide inactive spinner!";
            }
            else if (activeCount === 1) {
                activeCount--;
                hide();
            }
            else {
                activeCount--;
            }
        },
        clear: function(){
            activeCount = 0;
            hide();
        }
    };

    var $targetDiv = $(targetDiv);

    function initialize() {
        if ($targetDiv.length === 0) {
            throw "Spinner must be initialized with valid div!";
        }

        $targetDiv.css("display", "none");
        $targetDiv.css("position", "fixed");
    }

    initialize();

    return self;
}