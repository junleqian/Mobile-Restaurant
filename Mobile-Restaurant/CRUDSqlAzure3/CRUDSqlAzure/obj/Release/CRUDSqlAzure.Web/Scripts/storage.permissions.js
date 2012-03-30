/*
 * Functions and variables for Users Views
 *
 */
var outstandingRequests = new Array();

var processingRequests = false;

var changedIcon = "/Content/images/changed.png";
var clearIcon = "/Content/images/empty.png";
var activeIcon = "/Content/images/sending.gif";
var successIcon = "/Content/images/success.png";

function showIconForUser(userId, icon) {
    $("[id='" + userId + "_PendingChanges" + "']").attr("src", icon);
}

function pendingRequests() {
    return (outstandingRequests.length > 0);
}

function CommitChanges() {
    if (processingRequests) {
        alert("Wait until the previos changes have been applied to set new ones.");
        return;
    }
    if (!pendingRequests()) {
        alert("There are no pending changes to send to the server.");
        return;
    }

    /*
     * send request for the first user, the function will handle the daisy chaining of the rest
     */
    $('input[type=checkbox]').attr('disabled', 'true');
    processingRequests = true;
    sendChangesForUser(0);
}

function sendChangesForUser(offset) {
    if (offset >= outstandingRequests.length) {
        ClearAllRequests();
        $('input[type=checkbox]').removeAttr('disabled');
        processingRequests = false;
        return;
    }

    userId = outstandingRequests[offset];
    showIconForUser(userId, activeIcon);

    // SQL permissions
    var sqlRead = $("[id='Sql_Read" + userId + "']:checked").val() !== undefined;
    var sqlCreate = $("[id='Sql_Create" + userId + "']:checked").val() !== undefined;
    var sqlUpdate = $("[id='Sql_Update" + userId + "']:checked").val() !== undefined;
    var sqlDelete = $("[id='Sql_Delete" + userId + "']:checked").val() !== undefined;

    var permissions  = {
            'UserId': userId,
            'SqlRead': sqlRead,
            'SqlCreate': sqlCreate,
            'SqlUpdate': sqlUpdate,
            'SqlDelete': sqlDelete
    }

    $.ajax({
        url: actionUrl,
        cache: false,
        type: "POST",
        data: permissions,
        complete: function () {
            showIconForUser(userId, successIcon);
            sendChangesForUser(offset + 1);
        }
    });
}

function NotifyPermissionsChange(userId) {
    AddRequest(userId);
    showIconForUser(userId, changedIcon);
}
function AddRequest(userId, permissionNode) {
    for (var i = 0; i < outstandingRequests.length; i++) {
        if (outstandingRequests[i] == userId) {
            return;
        }
    }
    outstandingRequests[outstandingRequests.length] = userId;
}
function ClearAllRequests() {
    for (var i = 0; i < outstandingRequests.length; i++ ) {
        userId = outstandingRequests[i];
        showIconForUser(userId, clearIcon);
    }
    outstandingRequests = new Array();
}
/*******************************************************/

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

function SendMicrosoftNotification(containerId, userId, actionUrl) {
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
            SetMicrosoftPushNotificationImageStatus(containerId, "Result", "Error accessing Push Notification Service.");
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