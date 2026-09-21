/**
 * 功能描述(Description)：{PAGE_TITLE} 控制器脚本
 * 参考代码：03-PDA/LonSon.Mobile.PrinxChengShan.App/js/BarCodeUpdate.js
 */
var storage = window.localStorage;

(function($) {
    $.init();

    // 显示当前登录用户与所属工厂
    mui('#lblUser')[0].innerText = storage["NAME"] || "";
    mui('#lblTime')[0].innerText = "FTY." + (storage["FACNM"] || storage["FAC"] || "");

    var _selBARCODE = mui('#selBARCODE')[0];
    _selBARCODE.focus();

    // 扫码枪回车按键事件监听
    _selBARCODE.addEventListener('keyup', function() {
        if (13 == event.keyCode || 0 == event.keyCode) {
            var barcodeVal = _selBARCODE.value.trim();
            if (barcodeVal.length >= 6) {
                queryBarcodeInfo(barcodeVal);
            }
        }
        _selBARCODE.focus();
    }, false);

    // 确定按钮事件
    var btn = mui('#btnAdd')[0];
    btn.addEventListener('tap', function() {
        if (OnCheckText()) {
            mask.show();
            mui.ajax(requestPath + '/ashx/{MODULE_NAME}.ashx', {
                data: {
                    action: "up",
                    Token: storage["Token"],
                    lang: storage["Language"],
                    FAC: storage["FAC"],
                    LOGINNAM: storage["LOGINNAME"],
                    ENAM: storage["NAME"],
                    BARCODE: mui('#txtBARCODE')[0].value,
                    QTY: mui('#txtQTY')[0].value
                },
                dataType: 'json',
                type: 'post',
                timeout: 100000,
                success: function(data) {
                    if (data.ErrCode == "0") {
                        mui.toast(data.Error || "操作成功!", {
                            duration: tim,
                            type: 'div'
                        });
                        OnCleanText();
                    } else {
                        mui.alert(data.Error || "提交失败!", "Message");
                    }
                    _selBARCODE.value = '';
                    _selBARCODE.focus();
                    mask.close();
                },
                error: function(xhr, type) {
                    mask.close();
                    mui.alert("网络请求超时或异常: " + type, "错误");
                    _selBARCODE.focus();
                }
            });
        }
    }, false);

    /**
     * 扫码后查询条码基础信息
     */
    function queryBarcodeInfo(barcode) {
        mui.ajax(requestPath + '/ashx/{MODULE_NAME}.ashx', {
            data: {
                action: "by",
                Token: storage["Token"],
                FAC: storage["FAC"],
                LOGINNAM: storage["LOGINNAME"],
                ENAM: storage["NAME"],
                BARCODE: barcode,
                lang: storage["Language"]
            },
            dataType: 'json',
            type: 'post',
            success: function(data) {
                if (data.Info && data.Info.BARCODE) {
                    mui('#txtBARCODE')[0].value = data.Info.BARCODE;
                    mui('#txtITNBR')[0].value = data.Info.BUITNBR || "";
                    mui('#txtITDSC')[0].value = data.Info.BUITDSC || "";
                    mui('#txtQTY')[0].value = data.Info.QTY || "1";
                } else {
                    mui.toast("未找到扫描的条码信息!", {
                        duration: tim,
                        type: 'div'
                    });
                }
                _selBARCODE.value = '';
                _selBARCODE.focus();
            },
            error: function(xhr, type) {
                mui.toast("网络异常: " + type);
                _selBARCODE.focus();
            }
        });
    }

})(mui);

/**
 * 界面输入清空复位
 */
function OnCleanText() {
    mui('#txtBARCODE')[0].value = '';
    mui('#txtITNBR')[0].value = '';
    mui('#txtITDSC')[0].value = '';
    mui('#txtQTY')[0].value = '';
    var _sel = mui('#selBARCODE')[0];
    _sel.value = '';
    _sel.focus();
}

/**
 * 提交前必填项校验
 */
function OnCheckText() {
    if (!mui('#txtBARCODE')[0].value) {
        mui.alert("请先扫描条码信息！");
        mui('#selBARCODE')[0].focus();
        return false;
    }
    return true;
}
