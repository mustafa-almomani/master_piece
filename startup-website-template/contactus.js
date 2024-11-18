var url = "https://localhost:7072/api/contact/newmassege";
async function contactus() {
  event.preventDefault();
  var form = document.getElementById("formcontact");
  var formData = new FormData(form);

  var response = await fetch(url, {
    method: "POST",
    body: formData,
  });

  if (response.ok) {
    Swal.fire({
      title: "Success!",
      text: "message sent successfully",
      icon: "success",
      confirmButtonText: "OK",
      timer: 3000, // time in milliseconds
      timerProgressBar: true, // show a progress bar for the time
    });

    // Redirecting to the page after 2 seconds
    setTimeout(() => {
      window.location.reload();
    }, 2000);
  } else {
    Swal.fire({
      title: "Error!",
      text: "message sent  failed",
      icon: "error",
      confirmButtonText: "OK",
    });
  }
}

var url2 = "https://localhost:7072/api/contact/getallcontact";
async function getAllContact() {
  var response = await fetch(url2);
  var data = await response.json();
  var contact = document.getElementById("contact");

  // الحد الأقصى لعرض الرسالة في الكارد
  const messageLimit = 1; // الحد الأقصى لعدد الحروف

  data.forEach((element) => {
    // عرض جزء من الرسالة إذا كانت أطول من الحد المسموح
    let shortMessage = element.messageText;
    if (element.messageText.length > messageLimit) {
      shortMessage = element.messageText.substring(0, messageLimit) + "...";
    }

    // إنشاء الكارد
    // إنشاء الكارد
    contact.innerHTML += `
<div class="col-xl-4 col-md-6 mb-4">
  <div class="feedback-card" style="min-height: 300px; max-height: 400px; overflow: hidden; display: flex; flex-direction: column; justify-content: space-between;">
    <h5>${element.name}</h5>
    <div class="feedback-info">
      <p>
        <i class="fas fa-envelope"></i> ${element.email}
      </p>
      <h4>${element.phoneNumber}</h4>
    </div>
    <div class="feedback-message" style="overflow-y: auto; max-height: 100px; word-break: break-word;">
      <strong>Message:</strong> "${element.messageText}"
      <br>
      ${
        element.messageText.length > messageLimit
          ? '<a href="#" class="see-more" data-id="' +
            element.id +
            '" data-name="' +
            element.name +
            '" data-email="' +
            element.email +
            '" data-phone="' +
            element.phoneNumber +
            '" data-message="' +
            element.messageText.replace(/"/g, "&quot;") +
            '" data-bs-toggle="modal" data-bs-target="#messageModal">See More</a>'
          : ""
      }
    </div>
              <button onclick="deletemessege(${
                element.messageId
              })"" class="btn btn-danger btn-sm">
  <i class="fas fa-trash-alt"></i> Delete
</button>

    
  </div>
</div>
`;
  });

  // حدث للنقر على روابط "See More"
  const seeMoreLinks = document.querySelectorAll(".see-more");
  seeMoreLinks.forEach((link) => {
    link.addEventListener("click", (event) => {
      event.preventDefault();

      // جلب البيانات الكاملة من خصائص الرابط
      const name = link.getAttribute("data-name");
      const email = link.getAttribute("data-email");
      const phoneNumber = link.getAttribute("data-phone");
      const fullMessage = link.getAttribute("data-message");

      // عرض جميع البيانات في المودال
      document.getElementById("modalTitle").textContent = name;
      document.getElementById("modalEmail").value = email;
      document.getElementById("modalPhone").textContent = phoneNumber;
      document.getElementById("modalMessageContent").textContent = fullMessage;
    });
  });
}

getAllContact();

var urlsendemail = "https://localhost:7072/api/contact/adminRespone";
async function sendemail() {
  event.preventDefault();
  var form = document.getElementById("formcontactt");
  var formData = new FormData(form);

  var response = await fetch(urlsendemail, {
    method: "POST",
    body: formData,
  });

  if (response.ok) {
    Swal.fire({
      title: "Success!",
      text: "Email sent successfully",
      icon: "success",
      confirmButtonText: "OK",
      timer: 3000, // time in milliseconds
      timerProgressBar: true, // show a progress bar for the time
    });

    // Redirecting to the page after 2 seconds
    setTimeout(() => {
      window.location.reload();
    }, 2000);
  } else {
    Swal.fire({
      title: "Error!",
      text: "Email sent failed",
      icon: "error",
      confirmButtonText: "OK",
    });
  }
}

async function deletemessege(id) {
  const result = await Swal.fire({
    title: "Are you sure?",
    text: "This employee will be deleted!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#d33",
    cancelButtonColor: "#3085d6",
    confirmButtonText: "Yes, delete it!",
  });

  if (result.isConfirmed) {
    var delet = `https://localhost:7072/api/contact/deleteMessage/${id}`;
    var response = await fetch(delet, {
      method: "DELETE",
    });

    console.log("Response Status:", response.status);
    console.log("Response OK:", response.ok);

    if (response.ok) {
      await Swal.fire(
        "Deleted!",
        "The message has been deleted successfully.",
        "success"
      );
      location.reload();
    } else {
      const errorMessage = await response.text();
      await Swal.fire(
        "Error!",
        `There was an error deleting the message: ${errorMessage}`,
        "error"
      );
    }
  }
}

async function profile() {
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();
