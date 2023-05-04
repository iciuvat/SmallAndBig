function getItems() {
  fetch('api/all')
    .then(response => response.json())
    .then(data => _displayItems(data))
    .catch(error => console.error('Unable to get items.', error));
}

function _displayItems(data) {
  const tBody = document.getElementById('measurements_table');
  tBody.innerHTML = '';

  data.forEach(item => {
    let tr = tBody.insertRow();    
    tr.insertCell(0).appendChild(document.createTextNode(item.value));
    tr.insertCell(1).appendChild(document.createTextNode(item.unit));
    const date_time = new Date(item.timestamp);
    tr.insertCell(2).appendChild(document.createTextNode(date_time.toLocaleTimeString() + ' ' + date_time.toLocaleDateString()));
  });
}

getItems();
window.setInterval("getItems()", 5 * 1000);