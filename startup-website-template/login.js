const url = "https://localhost:7072/api/User";
async function register() {
  event.preventDefault();
  var form = document.getElementById("registerform");
  var formData = new FormData(form);
  var response = await fetch(url, {
    method: "POST",
    body: formData,
  });

  // localStorage.setItem("EmployeeId", Id);

  if (response.ok) {
    Swal.fire({
      title: "Success!",
      text: "Registration successful",
      icon: "success",
      confirmButtonText: "OK",
      timer: 3000, // 3 seconds
      timerProgressBar: true, // Show a progress bar
    });

    // Redirect to the page after 3 seconds
    setTimeout(() => {
      window.location.reload();
    }, 2000);
  } else {
    Swal.fire({
      title: "Error!",
      text: "Registration failed",
      icon: "error",
      confirmButtonText: "OK",
    });
  }
}

const url2 = "https://localhost:7072/api/User/LOGIN";
async function login() {
  event.preventDefault();
  var data = {
    password: document.getElementById("password").value,
    email: document.getElementById("email").value,
  };

  var response = await fetch(url2, {
    method: "POST",
    body: JSON.stringify(data),
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (response.ok) {
    var result = await response.json();
    console.log(result);

    if (result.role != null && result.role === "Admin") {
      localStorage.setItem("employeeId", result.employeeId);
      Swal.fire({
        title: "Success!",
        text: "Logged in as Admin.",
        icon: "success",
        timer: 3000,
        timerProgressBar: true,
      }).then(() => {
        window.location.href = "Employee.html";
      });
    } else if (result.role === "Employee") {
      localStorage.setItem("employeeId", result.employeeId);
      Swal.fire({
        title: "Success!",
        text: "Logged in as Employee.",
        icon: "success",
        timer: 3000,
        timerProgressBar: true,
      }).then(() => {
        window.location.href = "orderemployee.html";
      });
    } else {
      localStorage.setItem("UserID", result.userId);
      Swal.fire({
        title: "Success!",
        text: "Logged in successfully.",
        icon: "success",
        timer: 3000,
        timerProgressBar: true,
      }).then(() => {
        window.location.href = "index.html";
      });
    }
  } else {
    var errorMessage;
    if (response.status === 404) {
      errorMessage = "Email not found.";
    } else if (response.status === 401) {
      errorMessage = "Invalid password.";
    } else {
      errorMessage = "Login failed.";
    }

    Swal.fire({
      title: "Error!",
      text: errorMessage,
      icon: "error",
      confirmButtonText: "OK",
    });
  }
}
