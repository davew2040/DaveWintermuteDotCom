
var Griddlizer = new function (ulElement, config) {
    function GridConfigObject() {
        return {
            ColumnCount: 4,
            ItemProcessor: function (liElement) {
                return $(liElement).clone();
            },
            TableClass: null,
            TrClass: null,
            TdClass: null
        };
    }

    var self = {
        getConfigObject: function () {
            return new GridConfigObject();
        },
        buildItemGrid: function (ulElement, config) {
            var config = config || new GridConfigObject();

            var $ulElement = $(ulElement);

            var liElements = $($ulElement.find("li"));

            var totalRows = Math.ceil(liElements.length / config.ColumnCount);

            var newTable = $("<table></table>");
            if (config.TableClass !== null) {
                newTable.addClass(config.TableClass);
            }

            var cellCount = 0;
            var done = false;

            for (var row = 0; row < totalRows; row++) {
                if (done) { break; }

                var newTableRow = $("<tr></tr>");
                if (config.TrClass !== null) {
                    newTable.addClass(config.TrClass);
                }

                newTable.append(newTableRow);

                for (var col = 0; col < config.ColumnCount; col++) {
                    if (done) { break; }

                    var liElement = liElements[cellCount++];
                    if (cellCount >= liElements.length) {
                        done = true;
                    }

                    var tdElement = $("<td></td>");
                    if (config.TdClass !== null) {
                        newTable.addClass(config.TdClass);
                    }

                    var alternator = row + col;
                    if (alternator % 2 == 0) {
                        tdElement.addClass("evenCell");
                    }
                    else {
                        tdElement.addClass("oddCell");
                    }

                    tdElement.append(config.ItemProcessor($(liElement).clone()));

                    newTableRow.append(tdElement);
                }
            }

            return newTable;
        }
    };

    return self;
}();