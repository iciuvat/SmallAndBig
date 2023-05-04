function getLocations_and_measurements() {
  fetch('api/locations_and_measurements')
    .then(response => response.json())
    .then(data => _displayLocations(data))
    .catch(error => console.error('Unable to get items.', error));
}

function _displayLocations(data) {
  const locations_tables = document.getElementById('locations_tables');
  locations_tables.innerHTML = '';

  data.forEach(item => {
    var html;

    if (item.measurements == null) {
      html = `
      <div class="col mx-1 my-1">
          <table class="table table-bordered" width="400">
            <thead>
              <th colspan="3">
                ${item.id} <a href="${item.web}">${item.name}</a><br/>
                ${item.address}
              </th>
            </thead>
            <tbody>
            <tr>
                <td class="bg-danger text-white">offline</td>
            </tr></tbody>
          </table>
      </div>`
    }
    else {
      html = `
<div class="col mx-1 my-1">
    <table class="table table-bordered" width="400">
      <thead class="thead-light" id="measurements_thead_${item.id}">
        <tr>
            <th colspan="3">
              ${item.id} <a href="${item.web}">${item.name}</a><br/>
              ${item.address}
            </th>
        </tr>
        <tr>
            <th>Value</th>
            <th>Unit</th>
            <th>Date</th>
        </tr>
      </thead>
      <tbody>`;


      item.measurements.forEach(measurement => {
        const date_time = new Date(measurement.timestamp);
        html += `
        <tr>
          <td>${measurement.value}</td>
          <td>${measurement.unit}</td>
          <td>${ date_time.toLocaleTimeString() + ' ' + date_time.toLocaleDateString() }</td>
        </tr>`;
      });

      html += `
      </tbody>
    </table>
</div>`;
    }

    var template = document.createElement('template');
    html = html.trim();
    template.innerHTML = html;
    locations_tables.appendChild(template.content.firstChild);
  });
}

getLocations_and_measurements();
window.setInterval("getLocations_and_measurements()", 5 * 1000);