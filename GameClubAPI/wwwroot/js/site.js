const uriClubs = 'api/clubs';
let _clubPage = 1;
let _eventPage = 1;
let pageSize = 5;
let selectedClubId = '';

function getClubs() {
  fetch(`${uriClubs}?page=${_clubPage}`)
    .then(response => response.json())
    .then(data => _displayClubs(data))
    .catch(error => console.error('Unable to get items.', error));
}

function addClub() {
  const addClubNameTextbox = document.getElementById('add-club-name');
  const addClubDescriptionTextbox = document.getElementById('add-club-description');

  const item = {    
    name: addClubNameTextbox.value.trim(),
    description: addClubDescriptionTextbox.value.trim()
  };

  fetch(uriClubs, {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  })
    .then(response => response.json())
    .then(() => {
      _clubPage = 1;
      getClubs();
      addClubNameTextbox.value = '';
      addClubDescriptionTextbox.value = '';
    })
    .catch(error => console.error('Unable to add club.', error));
}

function closeInput() {
  document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
  document.getElementById('counter').innerText = `Total Clubs: ${itemCount}`;
}

function _displayClubs(data) {
  const tBody = document.getElementById('list-clubs');
  tBody.innerHTML = '';

  const total = data.total;
  const pageData = data.pageData;

  _displayCount(total);
  _displayPages(total);

  const button = document.createElement('button');

  pageData.forEach(item => {
    let viewEventButton = button.cloneNode(false);
    viewEventButton.innerText = 'View Events';
    viewEventButton.setAttribute('onclick', `viewClubEvents('${item.clubId}', '${item.name}')`);

    let tr = tBody.insertRow();
    
    let td2 = tr.insertCell(0);
    let nameTextNode = document.createTextNode(item.name);
    td2.appendChild(nameTextNode);

    let td3 = tr.insertCell(1);
    let descriptionTextNode = document.createTextNode(item.description);
    td3.appendChild(descriptionTextNode);

    let td4 = tr.insertCell(2);
    td4.appendChild(viewEventButton);
  });
}

function _displayPages(itemCount){
    const divClubPages = document.getElementById('clubPages');
    divClubPages.innerHTML = '';
    for(let i = 0; i < itemCount/pageSize; i++){
        var content = document.createElement('a');
        content.text = i + 1;
        content.setAttribute("onclick", `fetchClubs(${i + 1})`);
        if(i + 1 == _clubPage){
            content.setAttribute("class", "active");
        }
        else{
            content.setAttribute("class", "inactive");
        }
        divClubPages.appendChild(content)
    }
}

function fetchClubs(page){
    _clubPage = page;
    searchClub(_clubPage);
}

function seedClubs(){
    fetch(`${uriClubs}/seed`, {
        method: 'POST',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
      })
        .then(() => {
          getClubs();
        })
        .catch(error => console.error('Unable to seed clubs.', error));
}

function searchClub(page){
    if(!page){
        _clubPage = 1;
    }

    fetch(`${uriClubs}?page=${_clubPage}&keyword=${document.getElementById('searchClubKeyword').value.trim()}`)
    .then(response => response.json())
    .then(data => _displayClubs(data))
    .catch(error => console.error('Unable to get items.', error));
}

function viewClubEvents(clubId, clubName){
    selectedClubId = clubId;
    const eventClubText = document.getElementById('eventClubText');
    eventClubText.innerHTML = `Events of ${clubName}`;

    document.getElementById('divEvents').style.display = 'block';

    getEvents();
}

function seedEvents(){
    fetch(`${uriClubs}/${selectedClubId}/seedevents`, {
        method: 'POST',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
      })
        .then(() => {
            getEvents();
        })
        .catch(error => console.error('Unable to seed events.', error));
}

function getEvents() {
    fetch(`${uriClubs}/${selectedClubId}/events?page=${_eventPage}`)
      .then(response => response.json())
      .then(data => _displayEvents(data))
      .catch(error => console.error('Unable to get items.', error));
  }

 
function _displayEvents(data) {
    const tBody = document.getElementById('list-events');
    tBody.innerHTML = '';

    const total = data.total;
    const pageData = data.pageData;

    _displayCountEvent(total);
    _displayPageEvent(total);

    pageData.forEach(item => {
        let tr = tBody.insertRow();
        
        let td2 = tr.insertCell(0);
        let titleTextNode = document.createTextNode(item.title);
        td2.appendChild(titleTextNode);

        let td3 = tr.insertCell(1);
        let descriptionTextNode = document.createTextNode(item.description);
        td3.appendChild(descriptionTextNode);

        let td4 = tr.insertCell(2);
        let dateTextNode = document.createTextNode(item.scheduledDateTimeUtc);
        td4.appendChild(dateTextNode);
    });
  } 

  function _displayCountEvent(itemCount) {
    document.getElementById('counterEvents').innerText = `Total Events: ${itemCount}`;
  }
  
  function _displayPageEvent(itemCount){
    const divEventPages = document.getElementById('eventPages');
    divEventPages.innerHTML = '';
    for(let i = 0; i < itemCount/pageSize; i++){
        var content = document.createElement('a');
        content.text = i + 1;
        content.setAttribute("onclick", `fetchEvents(${i + 1})`);
        if(i + 1 == _eventPage){
            content.setAttribute("class", "active");
        }
        else{
            content.setAttribute("class", "inactive");
        }
        divEventPages.appendChild(content)
    }
}

function fetchEvents(page){
    _eventPage = page;
    getEvents();
}

function addEvent() {
    console.log('abc');
    const addEventTitleTextbox = document.getElementById('add-event-title');
    const addEventDescriptionTextbox = document.getElementById('add-event-description');
    const addEventDateTextbox = document.getElementById('add-event-date');
  
    const item = {    
      title: addEventTitleTextbox.value.trim(),
      description: addEventDescriptionTextbox.value.trim(),
      scheduledDateTimeUtc: addEventDateTextbox.value
    };
  
    fetch(`${uriClubs}/${selectedClubId}/events`, {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(item)
    })
      .then(response => response.json())
      .then(() => {
        _eventPage = 1;
        getEvents();
        addEventTitleTextbox.value = '';
        addEventDescriptionTextbox.value = '';
        addEventDateTextbox.value = '';
      })
      .catch(error => console.error('Unable to add event.', error));
  }
  