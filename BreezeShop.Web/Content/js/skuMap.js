

//保存最后的组合结果信息
var SKUResult = {};
//获得对象的key
function getObjKeys(obj) {
    if (obj !== Object(obj)) throw new TypeError('Invalid object');
    var keys = [];
    for (var key in obj)
        if (Object.prototype.hasOwnProperty.call(obj, key))
            keys[keys.length] = key;
    return keys;
}

//把组合的key放入结果集SKUResult
function add2SKUResult(combArrItem, sku) {
    var key = combArrItem.join(";");
    if (SKUResult[key]) {//SKU信息key属性·
        SKUResult[key].count += sku.count;
        SKUResult[key].prices.push(sku.price);
    } else {
        SKUResult[key] = {
            count: sku.count,
            prices: [sku.price]
        };
    }
}

//初始化得到结果集
function initSKU(data) {
    var i, j, skuKeys = getObjKeys(data);
    for (i = 0; i < skuKeys.length; i++) {
        var skuKey = skuKeys[i];//一条SKU信息key
        var sku = data[skuKey];	//一条SKU信息value
        var skuKeyAttrs = skuKey.split(";"); //SKU信息key属性值数组
        skuKeyAttrs.sort(function (value1, value2) {
            return parseInt(value1) - parseInt(value2);
        });

        //对每个SKU信息key属性值进行拆分组合
        var combArr = combInArray(skuKeyAttrs);
        for (j = 0; j < combArr.length; j++) {
            add2SKUResult(combArr[j], sku);
        }

        //结果集接放入SKUResult
        SKUResult[skuKeyAttrs.join(";")] = {
            count: sku.count,
            prices: [sku.price]
        }
    }
}

/**
 * 从数组中生成指定长度的组合
 * 方法: 先生成[0,1...]形式的数组, 然后根据0,1从原数组取元素，得到组合数组
 */
function combInArray(aData) {
    if (!aData || !aData.length) {
        return [];
    }

    var len = aData.length;
    var aResult = [];

    for (var n = 1; n < len; n++) {
        var aaFlags = getCombFlags(len, n);
        while (aaFlags.length) {
            var aFlag = aaFlags.shift();
            var aComb = [];
            for (var i = 0; i < len; i++) {
                aFlag[i] && aComb.push(aData[i]);
            }
            aResult.push(aComb);
        }
    }

    return aResult;
}


/**
 * 得到从 m 元素中取 n 元素的所有组合
 * 结果为[0,1...]形式的数组, 1表示选中，0表示不选
 */
function getCombFlags(m, n) {
    if (!n || n < 1) {
        return [];
    }

    var aResult = [];
    var aFlag = [];
    var bNext = true;
    var i, j, iCnt1;

    for (i = 0; i < m; i++) {
        aFlag[i] = i < n ? 1 : 0;
    }

    aResult.push(aFlag.concat());

    while (bNext) {
        iCnt1 = 0;
        for (i = 0; i < m - 1; i++) {
            if (aFlag[i] == 1 && aFlag[i + 1] == 0) {
                for (j = 0; j < i; j++) {
                    aFlag[j] = j < iCnt1 ? 1 : 0;
                }
                aFlag[i] = 0;
                aFlag[i + 1] = 1;
                var aTmp = aFlag.concat();
                aResult.push(aTmp);
                if (aTmp.slice(-n).join("").indexOf('0') == -1) {
                    bNext = false;
                }
                break;
            }
            aFlag[i] == 1 && iCnt1++;
        }
    }
    return aResult;
}

//skumap:SKU的json数据
//skucal:sku的选择框样式名
//disableCls:禁用的样式名
//selCls：选中的样式名
//specattr:SPECID的属性名字
//priceEle:显示的价格元素名字
//totalStock:总库存数量
//stockInput：随时变化的库存输入框名字
//change：每次点击后的变化事件
function skuExecute(skuMapData, skuCls, disableCls, selCls, specattr, priceEle, stockEle, totalStock, stockInput,change) {
    //原始价格
    var oPrice = $(priceEle).text();

    if (skuMapData == null || skuMapData.length <= 5) return;

    initSKU(skuMapData);

    $(skuCls).each(function () {
        var self = $(this);
        var attr_id = self.attr(specattr);
        if (!SKUResult[attr_id]) {
            self.addClass(disableCls);
        }
    }).click(function () {
        var self = $(this);

        //选中自己，兄弟节点取消选中
        self.toggleClass(selCls).siblings().removeClass(selCls);

        //已经选择的节点
        var selectedObjs = $(skuCls + '.' + selCls);

        console.log(selectedObjs);


        if (selectedObjs.length) {
            //获得组合key价格
            var selectedIds = [];
            selectedObjs.each(function () {
                selectedIds.push($(this).attr(specattr));
            });
            selectedIds.sort(function (value1, value2) {
                return parseInt(value1) - parseInt(value2);
            });
            var len = selectedIds.length;
            var prices = SKUResult[selectedIds.join(';')].prices;
            var maxStock = SKUResult[selectedIds.join(';')].count;
            var maxPrice = Math.max.apply(Math, prices);
            var minPrice = Math.min.apply(Math, prices);
            $(priceEle).text(maxPrice > minPrice ? minPrice + "-" + maxPrice : maxPrice);
            $(stockEle).text(maxStock);

            if (stockInput != undefined) {
                $(stockInput).attr("max", maxStock);
                //对库存的输入框处理
                if (parseInt($(stockInput).val()) > maxStock) {
                    $(stockInput).val(maxStock);
                }
            }

            //用已选中的节点验证待测试节点 underTestObjs
            $(skuCls).not(selectedObjs).not(self).each(function () {
                var siblingsSelectedObj = $(this).siblings('.' + selCls);
                var testAttrIds = [];//从选中节点中去掉选中的兄弟节点
                if (siblingsSelectedObj.length) {
                    var siblingsSelectedObjId = siblingsSelectedObj.attr(specattr);
                    for (var i = 0; i < len; i++) {
                        (selectedIds[i] != siblingsSelectedObjId) && testAttrIds.push(selectedIds[i]);
                    }
                } else {
                    testAttrIds = selectedIds.concat();
                }
                testAttrIds = testAttrIds.concat($(this).attr(specattr));
                testAttrIds.sort(function (value1, value2) {
                    return parseInt(value1) - parseInt(value2);
                });
                if (!SKUResult[testAttrIds.join(';')]) {
                    $(this).addClass(disableCls).removeClass(selCls);
                } else {
                    $(this).removeClass(disableCls);
                }
            });
        }
        else
        {
            //设置默认价格
            $(priceEle).text(oPrice);
            $(stockEle).text(totalStock);
            //设置属性状态
            $(skuCls).each(function () {
                SKUResult[$(this).attr(specattr)] ? $(this).removeClass(disableCls) : $(this).addClass(disableCls).removeClass(selCls);
            });

            if (stockInput != undefined) {
                $(stockInput).attr("max", totalStock);
            }
        }

        if (change != undefined) {
            change(this);
        }
    });
}