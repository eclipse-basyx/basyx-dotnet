function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}



function ToggleAutoReload(autoReloadCheckboxId, retrieveButtonId) {
    setTimeout(() => {
        if ($('#' + autoReloadCheckboxId).prop("checked")) {
            $('#' + retrieveButtonId).click();
            ToggleAutoReload(autoReloadCheckboxId, retrieveButtonId);
        }
    }, 1000);
}

function ExecuteOperation(requestPath, className) {
    var argNames = document.getElementsByClassName("argInName " + className);
    var argTypes = document.getElementsByClassName("argInType " + className);
    var argValues = document.getElementsByClassName("argInValue " + className);

    var args = [];
    for (var i = 0; i < argNames.length; i++) {
        var _valueType = argTypes.item(i).innerText;
        var _value = argValues.item(i).value;

        if (isNaN(_value) && (_valueType.includes('double') || _valueType.includes('float') || _valueType.includes('decimal'))) {
            _value = _value.replace(/,/g, '.')
        }

        var value = {
            idShort: argNames.item(i).innerText,
            modelType: {
                name: "Property"
            },
            valueType: _valueType,
            value: _value
        };
        var arg = {
            modelType: {
                name: "OperationVariable"
            },
            value
        };
        args.push(arg);
    }

    var invocationRequest = {
        requestId: uuidv4(),
        inputArguments: args,
        timeout: 60000
    }

    $('#executeIcon_' + className).children("i:first").remove()
    $('#executeIcon_' + className).addClass('spinner-border spinner-border-sm');

    $.ajax({
        type: 'POST',
        url: requestPath + '/invoke?async=false',
        contentType: 'application/json',
        data: JSON.stringify(invocationRequest),
        error: function (jqXHR, errorType, exception) {
            $('#executeIcon_' + className).removeClass('spinner-border spinner-border-sm');
            $('#executeIcon_' + className).html('<i class="fas fa-times"></i>');
            setTimeout(() => $('#executeIcon_' + className).children("i:first").remove(), 5000);
            alert("Failed to execute: '" + JSON.stringify(invocationRequest) + "' - Error: " + jqXHR.responseText + " | " + errorType + " | " + exception);
        },
        success: function (data) {
            $('#executeIcon_' + className).removeClass('spinner-border spinner-border-sm');
            for (var i = 0; i < data.outputArguments.length; i++) {
                var argOutIdShort = data.outputArguments[i].value.idShort;
                $('#argOutValue_' + className + argOutIdShort).val(JSON.stringify(data.outputArguments[i].value.value));
            }           
            if (data.executionResult.success) {
                $('#executeIcon_' + className).html('<i class="fas fa-check-circle"></i>');
                setTimeout(() => $('#executeIcon_' + className).children("i:first").remove(), 5000);
            } else {
                $('#executeIcon_' + className).html('<i class="fas fa-exclamation-triangle"></i>');
                setTimeout(() => $('#executeIcon_' + className).children("i:first").remove(), 5000);
            }
            if (data.executionResult.messages && data.executionResult.messages.length > 0) {
                $('#messageInput_' + className).val(JSON.stringify(data.executionResult.messages));
            }            
        }
    });
}


function GetPropertyValue(requestPath, hashedPathIdInput) {
    $.ajax({
        type: 'GET',
        url: requestPath + '?content=value',
        success: function (data) {
            $('#' + hashedPathIdInput).val(data);
            $('#' + hashedPathIdInput).css('border-color', 'green');
        }
    });
}


function ClearFields(hashedRequestPath) {
    $('.argInValue.' + hashedRequestPath).val('');
    $('.argOutValue.' + hashedRequestPath).val('');
    $('#messageInput_' + hashedRequestPath).val('');
}


function SetPropertyValue(requestPath, hashedPathIdInput, value, valueType) {
    if (isNaN(value) && (valueType.includes('double') || valueType.includes('float') || valueType.includes('decimal'))) {
        value = value.replace(/,/g, '.')
    }
    $.ajax({
        type: 'PUT',
        url: requestPath + '?content=value',
        contentType: 'application/json',
        data: JSON.stringify(value),
        error: function (jqXHR, errorType, exception) {
            $('#' + hashedPathIdInput).css('border-color', 'red');
            alert("Error updating Property-Value: '" + JSON.stringify(value) + "' - Error: " + jqXHR.responseText + " | " + errorType + " | " + exception);
        },
        statusCode: {
            204: function () {
                $('#' + hashedPathIdInput).css('border-color', 'green');
            }
        }
    });
}