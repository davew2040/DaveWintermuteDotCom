$(document).ready(function () {
    ResumeScript.initialize();
});

var ResumeScript = new function () {
    var self = {
        initialize: function () {
            $(".resumeContentBundle").each(function (index, element) {
                var widget = new HeaderWidget($(element));
                AnimationManager.registerAnimation(widget);
            });

            formatMulticolumnUlsAsTable();

            initializeResumeHeaders();
            initializeResumeExpandos();
        },
        lerpNumbers: function (n1, n2, percentage) {
            return n1 * (1.0 - percentage) + n2 * percentage;
        }
    };

    function buildTableFromUl($ul) {
        var config = Griddlizer.getConfigObject();
        config.TableClass = "skillsTable";
        config.ColumnCount = 2;

        var table = Griddlizer.buildItemGrid($ul, config);

        return table;
    }
    
    function formatMulticolumnUlsAsTable() {
        var $ul = $("ul.multiple_columns");
        $ul.each(function (index, element) {
            var result = buildTableFromUl($(element));
            $($ul.parent()).append(result);
            $ul.empty();
        });
    }

    function initializeResumeExpandos() {
        var $contentBlocks = $(".resumeContentDiv");
        $contentBlocks.each(function (index, element) {
            var $element = $(element);

            var expandoConfig = ExpandoBase.getConfigObject();
            expandoConfig.UseDynamicTiming = true;
            expandoConfig.StaticTransitionTime = 0.2;
            expandoConfig.InitialState = true;

            var expando = new Expando($element, expandoConfig);
        });
    }

    function initializeResumeHeaders() {
        var $headers = $(".clickableResumeHeader");
        $headers.each(function (index, item) {
            var $header = $(item);

            var $holderDiv = $("<div></div>");
            $holderDiv.addClass("clickableResumeHeaderHolder");
            $header.children().appendTo($holderDiv);
            $holderDiv.appendTo($header);

            var $minimizeDiv = $("<div class='minimize'></div>");
            $minimizeDiv.append("<span>[Minimize]</span>");
            $header.append($minimizeDiv);

            $header.click(function () {
                var data = GlobalUtilities.getElementDataObject($header[0]);
                if (!data.activeToggle) {
                    data.activeToggle = {
                        activeState: true
                    }
                }
                data.activeToggle.activeState = !data.activeToggle.activeState;

                var $textHolder = $header.find(".clickableResumeHeaderHolder");
                if (data.activeToggle.activeState) {
                    $header.removeClass("headerInactive");

                    $textHolder.css("left", "");
                }
                else {
                    $header.addClass("headerInactive");
                    
                    $textHolder.css("left", $header.width() - $textHolder.width()*0.7);
                }

                var $expando = $header.parent().find(".expando");
                if ($expando.length === 1) {
                    var expandoData = GlobalUtilities.getElementDataObject($expando[0]);
                    expandoData.expando.toggle();
                    $expando.scrollTop();
                }
            });
        });
    }

    return self;
}();

var HeaderWidget = function ($targetDiv) {
    var $targetDiv = $targetDiv;
    var animating = false;
    var activated = true;
    var animationTime = 0.5;
    var animationRate = 1.0 / animationTime;
    var animationTracker = 1.0;

    var borderActiveColor = ColorManager.getColorByHex6Code("#C0C0C0");
    var borderInactiveColor = ColorManager.getColorByHex6Code("#FF0000");
    var textActiveColor = ColorManager.getColorByHex6Code("#FFFFFF");
    //var textInactiveColor = ColorManager.getColorByHex6Code("#ddddc8");
    var textInactiveColor = ColorManager.getColorByHex6Code("#444444");
    var backgroundActiveColor = ColorManager.getColorByHex6Code("#BCC4F5");
    //var backgroundInactiveColor = ColorManager.getColorByHex6Code("#a4a393");
    var backgroundInactiveColor = ColorManager.getColorByHex6Code("#c4c4c4");
    var activeFontSize = 24.0;
    var inactiveFontSize = 16.0;

    function initialize() {
        setCss(1.0);
        $(getHeader()).append("<div class='headerMinimize'>[Minimize]</div>");
        $(getHeader()).click(clickHandler);
    }

    function updateState() {
        if (activated) {
            $(getHeader()).find(".headerMinimize").css("display", "inline");
        }
        else {
            $(getHeader()).find(".headerMinimize").css("display", "none");
        }
    }

    function clickHandler() {
        animating = true;
        activated = !activated;
        updateState();
    }
    
    function getHeader() {
        return $targetDiv.find("h2");
    }

    function update(delta) {
        if (!animating) {
            return;
        }

        if (activated) {
            animationTracker += delta * animationRate;
            if (animationTracker > 1.0) {
                setCss(1.0);
                animating = false;
                animationTracker = 1.0;
            }
            setCss(animationTracker);
        }
        else {
            animationTracker -= delta * animationRate;
            if (animationTracker < 0) {
                setCss(0.0);
                animating = false;
                animationTracker = 0.0;
            }
            setCss(animationTracker);
        }
    }

    function setCss(percentActive) {
        //var borderColor = ColorManager.lerpColors(borderInactiveColor, borderActiveColor, percentActive).GetHex();
        //var textColor = ColorManager.lerpColors(textInactiveColor, textActiveColor, percentActive).GetHex();
        //var backgroundColor = ColorManager.lerpColors(backgroundInactiveColor, backgroundActiveColor, percentActive).GetHex();

        //var $headerDiv = $(getHeader());

        //$headerDiv.css("border-color", borderColor);
        //$headerDiv.css("background-color", backgroundColor);
        //$headerDiv.css("color", textColor);
        //$headerDiv.css("font-size", "" + ResumeScript.lerpNumbers(inactiveFontSize, activeFontSize, percentActive) + "px");

        //var $divText = $headerDiv.find("span");

        //var MAGIC_MARGIN_ADJUSTMENT = 50;
        //var maxMargin = (1.0 - percentActive) * ($headerDiv.width() - $divText.width() - MAGIC_MARGIN_ADJUSTMENT)
        //$divText.css("margin-left", ResumeScript.lerpNumbers(maxMargin, 20.0, percentActive));

        //$divContent = $targetDiv.find(".resumeContentDiv");
        //$divActualContent = $targetDiv.find(".resumeContentSubDiv");

        //var MAGIC_SUBDIV_ADJUSTMENT = 20.0;
        //$divContent.css("height", "" + ResumeScript.lerpNumbers(0, $divActualContent.height() + MAGIC_SUBDIV_ADJUSTMENT, percentActive) + "px");

    }

    initialize();

    var self = {
        update: function (delta) {
            update(delta);
        }
    };

    return self;
};

var ColorManager = new function(){
    var Color = function()
    {
        var padString = function (number, length, padCharacter) {
            var numberString = number.toString();
            while (numberString.length < length) {
                numberString = padCharacter + numberString;
            }
            return numberString;
        }

        var self = { 
            Red: 0,
            Green: 0,
            Blue: 0,
            Alpha: 255,
            GetHex: function () {
                var red = self.Red.toString(16);
                var green = self.Green.toString(16);
                var blue = self.Blue.toString(16);
                return "#" + padString(red, 2, "0") + padString(green, 2, "0") + padString(blue, 2, "0");
            }
        }

        return self;
    };

    function convert2CharHexToByte(hex)
    {
        return parseInt(hex.substr(0, 1), 16)*16 + parseInt(hex.substr(1, 1), 16);
    }

    var self = {
        getColorByBytes: function(red, green, blue, alpha)
        {
            var newColor = new Color();
            newColor.Red = red;
            newColor.Green = green;
            newColor.Blue = blue;

            if (alpha !== undefined)
            {
                newColor.Alpha = alpha;
            }

            return newColor;
        },
        getColorByHex6Code: function (code) {
            if (code[0] === "#") {
                code = code.substr(1, 6);
            }

            var red = convert2CharHexToByte(code.substr(0, 2));
            var green = convert2CharHexToByte(code.substr(2, 2));
            var blue = convert2CharHexToByte(code.substr(4, 2));

            var newColor = new Color();
            newColor.Red = red;
            newColor.Green = green;
            newColor.Blue = blue;
            newColor.Alpha = 255;

            return newColor;
        },
        lerpColors: function(c1, c2, percentage)
        {
            if (percentage > 1.0) {
                percentage = 1.0;
            }
            else if (percentage < 0.0) {
                percentage = 0.0;
            }
            var c1percentage = (1.0-percentage);
            var newColor = new Color();

            newColor.Red = parseInt(c1.Red * c1percentage + c2.Red * percentage);
            newColor.Green = parseInt(c1.Green * c1percentage + c2.Green * percentage);
            newColor.Blue = parseInt(c1.Blue * c1percentage + c2.Blue * percentage);
            newColor.Alpha = parseInt(c1.Alpha * c1percentage + c2.Alpha * percentage);

            return newColor;
        }
    };

    return self;
}();