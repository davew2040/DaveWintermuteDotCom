var AuthorNewBlogPostScript = function () {
    var self = {
        initialize: function (jsonLinkString) {
            if (null !== jsonLinkString) {
                var splitString = jsonLinkString.split("|");

                for (var i = 0; i < splitString.length; i++) {
                    var linkString = splitString[i];
                    if (linkString.length > 0) {
                        addLink(linkString);
                    }
                }
            }

            $("#addLink").click(function () {
                addLink();
            });

            $("#submitter").click(function () {
                var $this = $(this);
                var form = $this.parents("form")[0];

                $(form["ImageLinks"]).val(buildLinks());

                form.submit();
            });
        }
    };

    var linkId = 1;

    function initializeLinkEvents(link) {
        var $link = $(link);
        $link.find(".linkRemover").click(function () {
            var $this = $(this);

            var link = $this.parents(".link");

            link.remove();
        });
    }

    function addLink(value) {
        var newLink = $("#linkTemplate .link").clone();
        newLink.css("display", "block");
        $("#links").append(newLink);
        newLink.attr("data-linkid", linkIdGenerator());

        if (value !== undefined) {
            newLink.find("input").val(value);
        }

        initializeLinkEvents(newLink);
    }

    function linkIdGenerator() {
        return linkId++;
    }

    function buildLinks() {
        var $links = $("#links input[type=text]");

        var returnString = "";

        for (var i = 0; i < $links.length; i++) {
            var $link = $($links[i]);

            returnString += $link.val() + "|";
        }

        return returnString;
    }

    return self;
}();