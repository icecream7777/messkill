/**
 * MES PDA 控制器脚本: {SCRIPT_NAME}.js
 * 负责条码扫描监听、输入合法性校验、WebService 数据交互与界面渲染
 */
(function($, doc) {
    mui.init();

    mui.plusReady(function() {
        var inputBarcode = doc.getElementById('BARCODE');
        var btnSubmit = doc.getElementById('btnSubmit');
        var btnClear = doc.getElementById('btnClear');

        // 默认光标聚焦至扫码框
        inputBarcode.focus();

        // 监听激光/红外扫码事件 (Enter 键 13 / 特殊键 0, 229)
        inputBarcode.addEventListener('keyup', function(event) {
            if (event.keyCode === 13 || event.keyCode === 0 || event.keyCode === 229) {
                var codeVal = inputBarcode.value.trim();
                if (codeVal.length >= 6) {
                    onBarcodeScanned(codeVal);
                }
            }
        });

        // 确认提交按钮
        btnSubmit.addEventListener('tap', function() {
            submitTransaction();
        });

        // 清空重置按钮
        btnClear.addEventListener('tap', function() {
            resetForm();
        });
    });

    /**
     * 条码扫入后的业务处理
     */
    function onBarcodeScanned(barcode) {
        var params = [
            storage["FAC"] || "02",
            storage["LOGINNAME"] || "",
            barcode
        ];

        mui.ajax(webUrl, {
            traditional: true,
            data: JSON.stringify({
                MethodName: "{SERVICE_PREFIX}_GetBarcodeInfo",
                Params: params
            }),
            dataType: 'json',
            type: 'post',
            timeout: 5000,
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            success: function(response) {
                var res = JSON.parse(response.d);
                if (res.Success && res.Data) {
                    doc.getElementById('ITNBR').value = res.Data.ITNBR || "";
                    doc.getElementById('ITDSC').value = res.Data.ITDSC || "";
                    doc.getElementById('QTY').value = res.Data.QTY || "1.000";
                    doc.getElementById('QTY').focus();
                } else {
                    mui.alert(res.Message || "未检索到该条码信息！", "提示", "确定", function() {
                        doc.getElementById('BARCODE').value = "";
                        doc.getElementById('BARCODE').focus();
                    });
                }
            },
            error: function(xhr, type) {
                mui.alert("网络请求失败: " + type, "错误");
            }
        });
    }

    /**
     * 提交操作
     */
    function submitTransaction() {
        var barcode = doc.getElementById('BARCODE').value.trim();
        var itnbr = doc.getElementById('ITNBR').value.trim();
        var qty = doc.getElementById('QTY').value.trim();

        if (!barcode) {
            mui.alert("请先扫描条码！", "提示");
            return;
        }

        var params = [
            storage["FAC"] || "02",
            storage["LOGINNAME"] || "",
            barcode,
            itnbr,
            qty
        ];

        mui.ajax(webUrl, {
            traditional: true,
            data: JSON.stringify({
                MethodName: "{SERVICE_PREFIX}_SubmitData",
                Params: params
            }),
            dataType: 'json',
            type: 'post',
            timeout: 6000,
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            success: function(response) {
                var res = JSON.parse(response.d);
                if (res.Success) {
                    mui.toast("提交成功！");
                    appendDetailRow(barcode, itnbr, qty);
                    resetForm();
                } else {
                    mui.alert(res.Message || "保存失败！", "错误");
                }
            },
            error: function(xhr, type) {
                mui.alert("提交异常: " + type, "错误");
            }
        });
    }

    /**
     * 将已处理记录添加到下方明细表格
     */
    function appendDetailRow(barcode, itnbr, qty) {
        var tbody = doc.getElementById('detailBody');
        var tr = doc.createElement('tr');
        var now = new Date();
        var timeStr = now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();

        tr.innerHTML = "<td>" + barcode + "</td>" +
                       "<td>" + itnbr + "</td>" +
                       "<td>" + qty + "</td>" +
                       "<td>" + timeStr + "</td>";
        tbody.insertBefore(tr, tbody.firstChild);

        var countSpan = doc.getElementById('recordCount');
        countSpan.innerText = parseInt(countSpan.innerText || "0") + 1;
    }

    /**
     * 重置表单并重新聚焦
     */
    function resetForm() {
        doc.getElementById('BARCODE').value = "";
        doc.getElementById('ITNBR').value = "";
        doc.getElementById('ITDSC').value = "";
        doc.getElementById('QTY').value = "";
        doc.getElementById('BARCODE').focus();
    }

})(mui, document);
