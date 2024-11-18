var employeeId = localStorage.getItem("employeeId");

async function getAllMeetings() {
  if (!employeeId) {
    console.error("EmployeeId not found in localStorage");
    return;
  }

  const url = `https://localhost:7072/api/User/getmeetingbyid${employeeId}`;

  try {
    const response = await fetch(url);

    if (!response.ok) {
      throw new Error("Failed to fetch meetings.");
    }

    const meeting = await response.json();

    const meetingsTableBody = document.getElementById("meetingsTableBody");

    meetingsTableBody.innerHTML = "";

    if (meeting) {
      meetingsTableBody.innerHTML += `
        <tr>
          <td>${meeting.meetingType || 'Team Meeting'}</td>
          <td>${new Date(meeting.meetingDateTime).toLocaleString()}</td>
          <td>${meeting.meetingAgenda}</td>
          <td><a href="${meeting.linkmeeting}" target="_blank">Zoom</a></td>
        </tr>
      `;
    } else {
      meetingsTableBody.innerHTML = "<tr><td colspan='4'>No meetings found.</td></tr>";
    }
  } catch (error) {
    console.error("Error fetching meetings:", error);
  }
}

getAllMeetings();


async function profile(){
    let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
    let response = await fetch(urlm);
    let data = await response.json();
    document.getElementById("nameprofile").innerHTML = data.firstName;
  
  }
  
  profile();