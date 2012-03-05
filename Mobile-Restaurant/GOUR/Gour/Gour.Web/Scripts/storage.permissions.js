function EnableContent(userId, actionUrl) {
    var useTables = $("[id='Tables_" + userId + "']:checked").val() !== undefined;
    var useBlobs = $("[id='Blobs_" + userId + "']:checked").val() !== undefined;
    var useQueues = $("[id='Queues_" + userId + "']:checked").val() !== undefined;
    var useSql = $("[id='Sql_" + userId + "']:checked").val() !== undefined;

    actionUrl = actionUrl.replace("_user_", encodeURIComponent(userId));
    actionUrl = actionUrl.replace("_useTables_", encodeURIComponent(useTables));
    actionUrl = actionUrl.replace("_useBlobs_", encodeURIComponent(useBlobs));
    actionUrl = actionUrl.replace("_useQueues_", encodeURIComponent(useQueues));
    actionUrl = actionUrl.replace("_useSql_", encodeURIComponent(useSql));

    $.ajax({ url: actionUrl, cache: false, type: "POST" });
}

function SetPublic(storage, actionUrl) {
    var isPublic = $("[id='Public_" + storage + "']").val() == "false";

    actionUrl = actionUrl.replace("_storage_", encodeURIComponent(storage));
    actionUrl = actionUrl.replace("_isPublic_", encodeURIComponent(isPublic));

    if (isPublic) {
        $("[id='Public_" + storage + "']").val("true");
        $("[id='Public_" + storage + "_Public']").removeClass("hidden");
        $("[id='Public_" + storage + "_Private']").addClass("hidden");
        $("[id='Public_" + storage + "_SetPublic']").addClass("hidden");
        $("[id='Public_" + storage + "_SetPrivate']").removeClass("hidden");
    } else {
        $("[id='Public_" + storage + "']").val("false");
        $("[id='Public_" + storage + "_Public']").addClass("hidden");
        $("[id='Public_" + storage + "_Private']").removeClass("hidden");
        $("[id='Public_" + storage + "_SetPublic']").removeClass("hidden");
        $("[id='Public_" + storage + "_SetPrivate']").addClass("hidden");
    }

    $("input[name^='" + storage + "'][type='checkbox']").each(function (ix, el) {
        el.disabled = isPublic;
    });

    $.ajax({ url: actionUrl, cache: false, type: "POST" });
}

function SetPermission(containerId, userId, storage, addPermissionActionUrl, removePermissionActionUrl) {
    var hasAccess = $("[id='" + storage + "_" + containerId + "']:checked").val() !== undefined;

    actionUrl = hasAccess ? addPermissionActionUrl : removePermissionActionUrl;

    actionUrl = actionUrl.replace("_storage_", encodeURIComponent(storage));
    actionUrl = actionUrl.replace("_user_", encodeURIComponent(userId));

    $.ajax({ url: actionUrl, cache: false, type: "POST" });
}

function SendMicrosoftNotification(containerId, userId, actionUrl, rawNotification) {
    var message = $("[id='Push_" + containerId + "_Message']").val();
    actionUrl = actionUrl.replace("_user_", encodeURIComponent(userId));
    actionUrl = actionUrl.replace("_message_", encodeURIComponent(message));

    SetMicrosoftPushNotificationImageStatus(containerId, "Sending");

    $.ajax({
        url: actionUrl,
        cache: false,
        type: "POST",
        success: function (data) {
            if (typeof data == "string") {
                var msg = "<img src=\"/Content/images/error_small.png\" alt=\"An error has ocurred while sending the push notification.\" title=\"An error has ocurred while sending the push notification.\" />";
                msg += " " + data;
                SetMicrosoftPushNotificationImageStatus(containerId, "Result", msg);
            } else {
                var msg = "";
                $.each(data, function (i, e) {
                    if (e.Status != "Success") {
                        msg += "<img src=\"/Content/images/error_small.png\" alt=\"An error has ocurred while sending the push notification.\" title=\"An error has ocurred while sending the push notification.\" />"
                        msg += " Device " + (i + 1) + " / " + data.length + " failed: " + e.Description + "<br />";
                    } else {
                        msg += "<img src=\"/Content/images/success_small.png\" alt=\"Notification successfully sent.\" title=\"Notification successfully sent.\" />"
                        msg += " Device " + (i + 1) + " / " + data.length + " success. <br />";
                    }
                });

                SetMicrosoftPushNotificationImageStatus(containerId, "Result", msg);
            }
        },
        error: function (data) {
            if (rawNotification) {
                SetMicrosoftPushNotificationImageStatus(containerId, "Result", "Error accessing Push Notification Service.");
            } else {
                if (message.match(/<script(.|\s)*?\/script>/g)) {
                    SetMicrosoftPushNotificationImageStatus(containerId, "Result", "You cannot send script code in a notification.");
                } else {
                    var tempDiv = document.createElement('div');
                    tempDiv.innerHTML = message.replace(/<script(.|\s)*?\/script>/g, '');
                    if (tempDiv.childNodes.length > 0) {
                        SetMicrosoftPushNotificationImageStatus(containerId, "Result", "You cannot send HTML in a notification.");
                    } else {
                        SetMicrosoftPushNotificationImageStatus(containerId, "Result", "Error accessing Push Notification Service.");
                    }
                }
            }
        }
    });
}

function SendAppleNotification(actionUrl) {
    var deviceId = $("#appleDeviceId").val();
    var message = $("#appleMessage").val();

    actionUrl = actionUrl.replace("_deviceId_", encodeURIComponent(deviceId));
    actionUrl = actionUrl.replace("_message_", encodeURIComponent(message));

    SetApplePushNotificationImageStatus("Sending");

    $.ajax({
        url: actionUrl,
        cache: false,
        type: "POST",
        success: function (data) {
            if ((typeof data == "string") && (data.toLowerCase() == "success")) {
                SetApplePushNotificationImageStatus("Success", "Notification successfully sent.");
            } else {
                SetApplePushNotificationImageStatus("Failed", data);
            }
        },
        error: function (data) {
            SetApplePushNotificationImageStatus("Failed", "Error accessing Apple Push Notification Service.");
        }
    });
}

function SetMicrosoftPushNotificationImageStatus(containerId, status, tooltip) {
    if (status == "Sending") {
        $("[id='Push_" + containerId + "_Sending']").show();
        $("[id='Push_" + containerId + "_Result']").html("");
    }
    else {
        $("[id='Push_" + containerId + "_Sending']").hide();
        $("[id='Push_" + containerId + "_Result']").html(tooltip);
    }
}

function SetApplePushNotificationImageStatus(status, tooltip) {
    if (status == "Sending") {
        $("#sendingNotificationStatus").show();
        $("#successNotificationStatus").hide();
        $("#failedNotificationStatus").hide();
        $("#messageStatus").hide();
        $("#messageStatus").html("");
    }
    else if (status == "Success") {
        $("#sendingNotificationStatus").hide();
        $("#successNotificationStatus").show();
        $("#failedNotificationStatus").hide();
        $("#messageStatus").html(tooltip);
        $("#messageStatus").show();
    }
    else {
        $("#sendingNotificationStatus").hide();
        $("#successNotificationStatus").hide();
        $("#failedNotificationStatus").show();
        $("#messageStatus").html(tooltip);
        $("#messageStatus").show();
    }
}