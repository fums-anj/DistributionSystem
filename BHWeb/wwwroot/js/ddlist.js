$(document).ready(function () {
    //CatalogDropDown();
    //ShopProductDropDown();
    $(function () {
        $("#CatalogId").change(function () {
            var Id = $(this).val();
            $.ajax(
                {
                    url: "/Inventory/StockTransfer/GetShopProductList/" + Id
                }
            ).done(function (Products) {
                $("#ShopProductId").html(Products);
            });
        });
    });

    $(function () {
        $("#ShopProductId").change(function () {
            var Id = $(this).val();
            $.ajax(
                {
                    url: "/Inventory/StockTransfer/GetVariantList/" + Id
                }
            ).done(function (items) {
                $("#VariantId").html(items);
            });
        });
    });
    
    

    $(function () {
        $("#VariantId").change(function () {
            var Id = $(this).val();
            $.ajax(
                {
                    url: "/Inventory/StockTransfer/GetUnitOfMeasureList/" + Id
                }
            ).done(function (Units) {
                $("#UnitOfMeasureId").html(Units);
            });
        });
    });

});

