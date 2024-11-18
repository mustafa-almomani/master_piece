var urlemployee = "https://localhost:7072/api/taskes/getemployeesperformance";
var employeeData = [];

async function getalldata() {
  var response = await fetch(urlemployee);
  employeeData = await response.json();
  displayData(employeeData);
}

// دالة لعرض البيانات في الجدول
function displayData(data) {
  var contaner = document.getElementById("contaner");
  contaner.innerHTML = ""; // مسح المحتوى القديم

  data.forEach((element) => {
    contaner.innerHTML += `
        <tr>
            <td>${element.employeeName}</td>
            <td>${element.email}</td>
            <td>${element.role}</td>
            <td>${element.completedTasks}</td>
            <td><span class="badge bg-success">${element.performanceRating}</span></td>
            <td><button class="btn btn-info btn-sm" data-bs-toggle="modal" data-bs-target="#performanceModal-${element.employeeId}">View Details</button></td>
        </tr>
        
        <!-- Modal لكل موظف -->
        <div class="modal fade" id="performanceModal-${element.employeeId}" tabindex="-1" aria-labelledby="performanceModalLabel-${element.employeeId}" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="performanceModalLabel-${element.employeeId}">Performance Details - ${element.employeeName}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p><strong>Completed Tasks:</strong> ${element.completedTasks}</p>
                        <p><strong>Tasks Range:</strong> ${element.performanceReport.tasksRange}</p>
                        <p><strong>Notes:</strong> ${element.performanceReport.generalReport}</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
        `;
  });
}

// دالة البحث
function searchData() {
  var input = document.getElementById("searchInput").value.toLowerCase();

  var filteredData = employeeData.filter((element) => {
    return (
      element.employeeName.toLowerCase().includes(input) ||
      element.completedTasks.toString().includes(input)
    );
  });

  displayData(filteredData); // عرض البيانات المفلترة
}

getalldata();

async function profile() {
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();
