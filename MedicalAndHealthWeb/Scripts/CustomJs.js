
/*Add Desire*/
function AddReferenceToSession() {

    var refName = $("#RefName").val();
    var refPost = $("#RefPost").val();

    var dataParam = { ReferenceName: refName, ReferencePost: refPost };
    jQuery.ajax({
        type: "POST",
        url: '/Desire/AddDesireRefrence',
        
        contentType: 'application/json',
        data: JSON.stringify(dataParam),
        success: function (result) {
            if (result == "AlreadyExist") {
                alert("Reference already added with thin name and Post.\nPlease enter difference reference.");
            }
            else if(result=="InvalidInput")
            {
                alert("Please enter valid Reference Name and Post.");
            }
            else {
                RefereshReferenceeList(result);
            }
        }
    });

}
/*Delete/Remove Desire*/
function RemoveReferenceFromSession(btnremove) {

    var row = btnremove.closest("tr");

    var refName = $(row).find("td").eq(0).text();
    var refPost =   $(row).find("td").eq(1).text();
  
    var dataParam = { ReferenceName: refName, ReferencePost: refPost };
    jQuery.ajax({
        type: "POST",
        url: '/Desire/DelteDesireRefrence',
       
        contentType: 'application/json',
        data: JSON.stringify(dataParam),
        
        success: function (result) {

            RefereshReferenceeList(result);
        }
    });

}
/*Refresh Desire's reference List*/
function RefereshReferenceeList(rdata) {
   
    var table = $("#referenceGrid table tbody");
    table.html("");
    var removeBtn= "<button type='button' title='Remove' class='btn btn-default' onclick='RemoveReferenceFromSession(this)'>Remove</button>";
    for (var i = 0; i < rdata.length; i++) { 
        var tablecol = '<td>' + rdata[i].ReferenceName + '</td><td>' + rdata[i].ReferencePost + '</td><td>' + removeBtn + '</td>';
            table.append('<tr>' + tablecol + '</tr>');  
        } 
}

/*Filter Desire Grid*/
function FilterGrid() {

    var selectedMLA = $("#fltMLA").val();
    var selectedDept = $("#fltDepartments").val();
    var selectedPost= $("#fltPost").val();
    var selectedEmp = $("#fltEmployees").val();
    var spec = "";
    var isDispatchedCheck = false;//$("#fltEmployees").val();

    var url = window.location.pathname.split("/");
    var controller = url[1];
    if (controller == "")
        controller = "Desire";
    $divListItem = $('#divListView');
    alert(controller.toLowerCase());
    if (controller.toLowerCase() == "report")
        url = '/' + controller + '/GetFilteredDesires?referenceName=' + selectedMLA + '&department=' + selectedDept + '&post=' + selectedPost + '&empName=' + selectedEmp + '&isDispatchedDesires=' + isDispatchedCheck;
    else 
        url = '/' + controller + '/GetFilteredDesires?referenceName=' + selectedMLA + '&department=' + selectedDept + '&post=' + selectedPost + '&empName=' + selectedEmp + '&speciality=' + spec;

    console.log(url);

    $.get(url, function (data) {
        if (data) {
            $divListItem.html(data);
        } else {
            location.reload();
        }
    });

}


function GetDispatchDesires() {

 
    var url = window.location.pathname.split("/");
    var controller = url[1];

    $divListItem = $('#divDispatchDesires');
    url = '/' + controller + '/GetCurrentDispatchDesire';

    console.log(url);

    $.get(url, function (data) {
        if (data) {
            RefereshDispatcheList(data);
            $("#desireToDispatchList").show();
 
        } else {
            location.reload();
        }
    });

}
function RefereshDispatcheList(data) {

    console.log(data);
    var table = $("#divDispatchDesires table tbody");
    table.html("");
    var removeBtn = "<button type='button' title='Remove' class='btn btn-default' onclick='RemoveDispatchedDesireFromSession(this)'>Remove</button>";
    for (var i = 0; i < data.length; i++) {
        var tablecol = '<td class=>' + data[i].ID + '</td><td>' + data[i].NameOfEmployee + '</td><td>' + data[i].Post + '</td><td>' + data[i].CurrentLocation + '</td><td>' + data[i].DesireLocation + '</td><td>' + removeBtn + '</td>';
        table.append('<tr>' + tablecol + '</tr>');
    }
}
function RemoveDispatchedDesireFromSession(btnremove) {

    var row = btnremove.closest("tr");

    var desireId = $(row).find("td").eq(0).text();
    
    var dataParam = { DesireId: desireId };
    jQuery.ajax({
        type: "POST",
        url: '/DispatchAndOrder/DeleteDispatchDesireFromSession',

        contentType: 'application/json',
        data: JSON.stringify(dataParam),

        success: function (result) {
            if(result=="Done")
                GetDispatchDesires();
        }
    });

}
function AddDesireToDispatch(id) {

   
    var dispatchDate = $("#txtDispatchDate").val();
    var dispatchNo = $("#textDispatchNo").val();
    var dispatchTo = $("#txtDispatchTo").val();
    var dataParam = { ID: id, DispatchDate: dispatchDate, DispatchNumber: dispatchNo, DispatchTo: dispatchTo };
     
    var url = window.location.pathname.split("/");
    var controller = url[1];
    jQuery.ajax({
        type: "POST",
        url: '/' + controller + '/AddDesireToDispatch',

        contentType: 'application/json',
        data: JSON.stringify(dataParam),
        success: function (result) { 
            window.location.href = "/" + controller + "/DispatchedDesire"; 
        }
    });

}
 
function AddEditDispatchInfo(id) { 

    var dataParam = { ID: id };
    var url = window.location.pathname.split("/");
    var controller = url[1];
    jQuery.ajax({
        type: "POST",
        url: '/DispatchAndOrder/AddEditDispatchInfo',

        contentType: 'application/json',
        data: JSON.stringify(dataParam),
        success: function (result) {
            window.location.href = "/DispatchAndOrder/DispatchedDesire";
        }
    });

}
function SubmitDispatchInfo() {

    var dispatchDate = $("#txtDispatchDate").val();
    var dispatchNo= $("#textDispatchNo").val();
    var dispatchTo = $("#txtDispatchTo").val();


    var dataParam = { DispatchDate: dispatchDate, DispatchNumber: dispatchNo, DispatchTo: dispatchTo };
    var url = window.location.pathname.split("/");
    var controller = url[1];
    jQuery.ajax({
        type: "POST",
        url: '/' + controller + '/SubmitDispatchInfo',

        contentType: 'application/json',
        data: JSON.stringify(dataParam),
        success: function (result) { 
            if (result == "error") {
                alert("Dispatch detail could not saved, There might be some error,\nPlease contact to your agent");
            } else {
                alert("Dispatched Successfully.");
            }
        }
    });

}

function FilterDispatchedGrid() {

    var dispatachToVal = $("#DispatchTo").val();
    var dispatchNoVal = $("#textDispatchNo").val();
 

    var url = window.location.pathname.split("/");
    var controller = url[1];
    alert(dispatachToVal + ",  " + dispatchNoVal);
    $divListItem = $('#divListView');
    url = '/' + controller + '/GetFilteredDispatchList?dispatchTo=' + dispatachToVal + '&dispatchNo=' + dispatchNoVal;

    console.log(url);

    $.get(url, function (data) {
        if (data) {
            $divListItem.html(data);
        } else {
            location.reload();
        }
    });

}
/*Method to Delete Desire*/
function DeleteDesire(id) {
    var dataParam = { ID: id };
    var confirmToDelete = confirm("Are you sure, you want to delete this desire?");
    var url = window.location.pathname.split("/");
    var controller = url[1];
    if (confirmToDelete) {
        jQuery.ajax({
            type: "POST",
            url: '/Desire/DeleteDesire',
            contentType: 'application/json',
            data: JSON.stringify(dataParam),
            success: function (result) {
                if (result == "error") {
                    alert("Desire could not deleted, There might be some error,\nPlease contact to your nodal officer");
                } else {
                    alert("Desire deleted Successfully.");
                    window.location.href = "/Desire/Index";
                }
            }
        });
    }
}
/*Method to Delete Dispathced Desire*/
function DeleteDispatchedDesire(id) {
    var dataParam = { ID: id };
    var confirmToDelete = confirm("Are you sure, you want to delete this Dispatched desire?");
    var url = window.location.pathname.split("/");
    var controller = url[1];
    if (confirmToDelete) {
        jQuery.ajax({
            type: "POST",
            url: '/DispatchAndOrder/DeleteDispatchedDesire',
            contentType: 'application/json',
            data: JSON.stringify(dataParam),
            success: function (result) {
                if (result == "error") {
                    alert("Could not deleted, There might be some error,\nPlease contact to your nodal officer");
                } else {
                    alert("Dispatched desire deleted Successfully.");
                    window.location.href = "/DispatchAndOrder/Index";
                }
            }
        });
    }
}
/*Get all selected desire ids to export*/

function GetSelectDesireToPrint() {
    var divReportList = $("#divReportListView");
    var selected = [];
    $('#divReportListView input:checked').each(function () {
        selected.push($(this).attr('id'));
    });
    var selectedIds = "";
    for (var i = 0; i < selected.length; i++)
    {
        selectedIds = selectedIds+selected[i] + ",";
    }
    alert(selectedIds);
    var dataParam = { selectedIds: selectedIds };
    if (selectedIds != "") {
        jQuery.ajax({
            type: "POST",
            url: '/Report/ExportData',
            contentType: 'application/json',
            data: JSON.stringify(dataParam),
            success: function (result) {
                if (result == "error") {
                    //alert("Could not deleted, There might be some error,\nPlease contact to your nodal officer");
                } else {
                    //alert("Dispatched desire deleted Successfully.");
                   // window.location.href = "/DispatchAndOrder/Index";
                }
            }
        });
    }
   
    return false;
}