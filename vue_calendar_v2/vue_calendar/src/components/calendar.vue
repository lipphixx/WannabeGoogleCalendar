<script setup>
import Day from "@/components/day.vue";
import {computed, onMounted, reactive, ref, watch} from "vue";
import axios from "axios";

const currentYear = ref(0);
const currentMonth = ref(0);
const daysInCurrentMonth = ref(0);

const daysBefore = ref([]);
const daysCurrent = ref([]);
const daysAfter = ref([]);

const events = reactive({});

const addEventDialog = ref(null);
const selectedDay = ref(null);
const selectedDate = ref(null);

const selectedEvent = ref(null);
const selectedEventParticipants = ref(null);
const eventDialog = ref(null);

const eventName = ref(null);

const eventTime = ref(null);
const isTimeOn = ref(false);
const isHostOn = ref(false);
const isNoteOn = ref(false);

const eventNote = ref(null);

const currentParticipant = ref(null);
const participants = ref(null);
const participantsIds = ref(null);
const participantsEmails = ref(null);
const participantsIncluded = ref(null);

const nameInp = ref(null);
const dateInp = ref(null);
const timeInp = ref(null);
const noteInp = ref(null);

const onEdit = ref(false);
const loggedUserId = ref(null);

const nationalHolidays = ref([]);

const today = new Date();

const props = defineProps(['loggedUser', 'action', 'loggedUsername', 'showEvents', 'showHolidays', 'createEvent']);
const emit = defineEmits(['onLoading'])

const filteredEvents = computed(() => {
  if (props.showEvents) {
    return events;
  } else {
    return {};
  }
});
const filteredHolidays = computed(() => {
  if (props.showHolidays) {
    return nationalHolidays.value;
  } else {
    return [];
  }
});
const isToday = (day) => {
  return day.getDate() === today.getDate() &&
      day.getMonth() === today.getMonth() &&
      day.getFullYear() === today.getFullYear();
}
onMounted(async () => {
  loggedUserId.value = props.loggedUser;
  const date = new Date();
  currentMonth.value = date.getMonth();
  currentYear.value = date.getFullYear();
  daysInCurrentMonth.value = new Date(currentYear.value, currentMonth.value + 1, 0).getDate();

  await fetchEvents();

  sortAllEvents();
  await fetchHolidays();
  drawCalendar();
});
watch(props.action, () => {
  if (props.action.action === 0) {
    showNow();
  } else if (props.action.action === 1) {
    previousMonth();
  } else if (props.action.action === 2) {
    nextMonth()
  } else if (props.action.action === 3) {
    showNow();
  }
})
watch(props.createEvent, () => {
  if (props.createEvent.action === true) {
    showModal(new Date());
  }
})

function drawCalendar() {
  daysInCurrentMonth.value = new Date(currentYear.value, currentMonth.value + 1, 0).getDate();
  let firstDayNum = new Date(currentYear.value, currentMonth.value, 0).getDay();
  let currentDay = 1;

  let prevMonth = currentMonth.value - 1;
  let workingYear = currentYear.value;
  if (prevMonth < 0) {
    prevMonth = 11;
    workingYear--;
  }

  let daysInPrevMonth = new Date(workingYear, currentMonth.value, 0).getDate();

  daysBefore.value = [];
  daysAfter.value = [];
  daysCurrent.value = [];

  for (let i = daysInPrevMonth - firstDayNum; i < daysInPrevMonth; i++) {
    daysBefore.value.push(new Date(workingYear, prevMonth, i + 1));
  }

  for (let j = 0; j < daysInCurrentMonth.value; j++) {
    daysCurrent.value.push(new Date(currentYear.value, currentMonth.value, currentDay));
    currentDay++;
  }

  let nextMonth = currentMonth.value + 1;
  let sussyYear = currentYear.value;
  if (nextMonth > 11) {
    nextMonth = 0;
    sussyYear++;
  }
  let daysToSeven = 7 - new Date(currentYear.value, currentMonth.value + 1, 0).getDay();
  for (let z = 0; z < daysToSeven; z++) {
    daysAfter.value.push(new Date(sussyYear, nextMonth, z + 1));
  }
  fetchHolidays()
}

function showModal(day) {
  isTimeOn.value = false;
  isHostOn.value = false;
  isNoteOn.value = false;
  eventName.value = null;
  eventTime.value = null;
  eventNote.value = null;
  currentParticipant.value = null;
  participants.value = null;
  participantsIds.value = null;
  participantsEmails.value = null;

  selectedDay.value = day;
  selectedDate.value = new Date(day.getFullYear(), day.getMonth(), day.getDate() + 1).toISOString().split("T")[0];
  addEventDialog.value.showModal();
}

async function sendEmail() {
  let date = `${selectedDay.value.getDate().toString().padStart(2, '0')}.${(selectedDay.value.getMonth() + 1).toString().padStart(2, '0')}.${selectedDay.value.getFullYear()}`;
  let emailDetails;
  for (const x of participants.value) {
    if (eventTime.value !== null) {
      emailDetails = {
        emailToId: x.email,
        emailToName: x.fullName,
        emailSubject: `Pozvánka na událost ${eventName.value}`,
        emailBody: `Byl(a) jste pozván(a) k účasti na události ${eventName.value}, která se koná ${date} v ${eventTime.value}.
      <br>
      Podrobnosti o události:<br>
      Název události: ${eventName.value}<br>
      Datum události: ${date}<br>
      Událost vytvořil: ${props.loggedUsername}<br>
      Poznámka: ${eventNote.value}`
      }
    } else {
      emailDetails = {
        emailToId: x.email,
        emailToName: x.fullName,
        emailSubject: `Pozvánka na událost ${eventName.value}`,
        emailBody: `Byl(a) jste pozván(a) k účasti na události ${eventName.value}, která se koná ${date}.
      <br>
      <br>
      Podrobnosti o události:<br>
      Název události: ${eventName.value}<br>
      Datum události: ${date}<br>
      Událost vytvořil: ${props.loggedUsername}<br>
      Poznámka: ${eventNote.value}`
      }
    }
    try {
      await axios.post(`https://localhost:5050/api/Mail`, emailDetails);
    } catch (error) {
      console.error(`Chyba při odesílání e-mailu na ${x.email}:`, error);
    }
  }
}

async function sendEmailUpdate(name, date, time, note, owner) {
  //let dateToSend = `${date.getDate().toString().padStart(2, '0')}.${(date.getMonth() + 1).toString().padStart(2, '0')}.${date.getFullYear()}`;
  let emailDetails;
  for (const x of participants.value) {
    if (time !== null) {
      emailDetails = {
        emailToId: x.email,
        emailToName: x.fullName,
        emailSubject: `Upravena událost ${name}`,
        emailBody: `Událost ${name} byla upravena.
      <br>
      Podrobnosti o události:<br>
      Název události: ${name}<br>
      Datum události: ${date}<br>
      Čas události: ${time}<br>
      Událost vytvořil: ${owner}<br>
      ${note ? `Poznámka: ${note}` : ''}`
      }
    } else {
      emailDetails = {
        emailToId: x.email,
        emailToName: x.fullName,
        emailSubject: `Upravena událost ${eventName.value}`,
        emailBody: `Událost ${eventName.value} byla upravena.
      <br>
      <br>
      Podrobnosti o události:<br>
      Název události: ${name}<br>
      Datum události: ${date}<br>
      Událost vytvořil: ${owner}<br>
      ${note ? `Poznámka: ${note}` : ''}`
      }
    }
    try {
      await axios.post(`https://localhost:5050/api/Mail`, emailDetails);
    } catch (error) {
      console.error(`Chyba při odesílání e-mailu na ${x.email}:`, error);
    }
  }
}

async function addEvent() {
  let newEvent;
  selectedDay.value = new Date(selectedDate.value);
  console.log(new Date(selectedDate.value));
  const key = selectedDay.value.getTime();

  if (!eventName.value) {
    alert('Zadejte název události!');
    return;
  }
  if (!eventTime.value && !isTimeOn) {
    alert('Zadejte čas nebo vyberte "Celý den"!');
    return;
  }

  if (!events[key]) {
    events[key] = [];
  }

  const eventDetails = {
    name: eventName.value,
  };

  if (isTimeOn && eventTime.value) {
    eventDetails.time = eventTime.value;
  }

  events[key].push(eventDetails);
  sortEventsForDay(events[key]);

  if (!Array.isArray(participantsIds.value)) {
    participantsIds.value = [];
  }

  if (participantsIds.value.length !== 0) {
    newEvent = {
      eventName: eventName.value,
      eventDate: `${selectedDay.value.getFullYear()}-${(selectedDay.value.getMonth() + 1).toString().padStart(2, '0')}-${selectedDay.value.getDate().toString().padStart(2, '0')}`,
      eventTime: eventTime.value ? `${eventTime.value}:00` : null,
      eventNote: eventNote.value,
      ownerId: props.loggedUser,
      participantsIds: JSON.stringify(participantsIds.value)
    };
    await sendEmail();
  } else {
    newEvent = {
      eventName: eventName.value,
      eventDate: `${selectedDay.value.getFullYear()}-${(selectedDay.value.getMonth() + 1).toString().padStart(2, '0')}-${selectedDay.value.getDate().toString().padStart(2, '0')}`,
      eventTime: eventTime.value ? `${eventTime.value}:00` : null,
      eventNote: eventNote.value,
      ownerId: props.loggedUser,
      participantsIds: null
    };
  }

  eventName.value = null;
  eventTime.value = null;
  eventNote.value = null;
  isTimeOn.value = false;
  participantsIds.value = null;

  addEventDialog.value.close();

  await axios.post(`https://localhost:5050/api/Events/CreateNewEvent`, newEvent);

  await resetPage();
}

async function addParticipant() {
  if (!Array.isArray(participantsIds.value)) {
    participantsIds.value = [];
  }
  if (!Array.isArray(participantsEmails.value)) {
    participantsEmails.value = [];
  }
  if (!Array.isArray(participants.value)) {
    participants.value = [];
  }

  if (currentParticipant.value !== null) {

    const response = await fetch(`https://localhost:5050/api/Users`);
    const data = await response.json();

    const participant = (data.find(x => x.email === currentParticipant.value));

    participants.value.push(participant);
    participantsIds.value.push(participant.userId);
    participantsEmails.value.push(participant.email);

    currentParticipant.value = null;
  }
}

function nextMonth() {
  if (currentMonth.value === 11) {
    currentMonth.value = 0;
    currentYear.value++;
  } else {
    currentMonth.value++;
  }
  drawCalendar();
}

function previousMonth() {
  if (currentMonth.value === 0) {
    currentMonth.value = 11;
    currentYear.value--;
  } else {
    currentMonth.value--;
  }
  drawCalendar();
}

function showNow() {
  const now = new Date();
  currentMonth.value = now.getMonth();
  currentYear.value = now.getFullYear();
  drawCalendar();
}

async function removeEvent(day, index) {
  const key = events[new Date(day).getTime()].findIndex(x => x.id === index);
  events[new Date(day).getTime()].splice(key, 1);
  await axios.delete(`https://localhost:5050/api/Events/${index}`);
  await resetPage();
}

function sortEventsForDay(dayEvents) {
  dayEvents.sort((a, b) => {
    if (!a.time && b.time) return -1; //celodenni event
    if (a.time && !b.time) return 1; //event s casem
    if (a.time && b.time) return a.time.localeCompare(b.time);
    return 0;
  });
}

function sortAllEvents() {
  //prochazime vsechny dny
  for (let day in events) {
    sortEventsForDay(events[day]); //seradime vsechny eventy pro dany den
  }
}

async function fetchHolidays() {
  const baseUrl = "https://svatkyapi.cz/api/day/";

  const formattedMonth = currentMonth.value < 10 ? `0${currentMonth.value + 1}` : `${currentMonth.value + 1}`;

  const fullUrl = `${baseUrl}${currentYear.value}-${formattedMonth}-01/interval/${daysInCurrentMonth.value}`;

  try {
    const response = await fetch(fullUrl);
    const data = await response.json();

    nationalHolidays.value = data.map(x => ({
      id: x.date,
      name: x.name
    }))

  } catch (error) {
    console.log(error);
  }

}

async function fetchEvents() {
  const url = `https://localhost:5050/api/Events/GetAllEvents`;

  try {
    const response = await fetch(url);
    const data = await response.json();

    const ownerEvents = [];
    data.find(x => {
      if (String(x.ownerId) === String(loggedUserId.value)) {
        ownerEvents.push(x);
      }
    });

    const participantEvents = data.filter(x => {
      if (!x.participantsIds) return false;
      return x.participantsIds.includes(loggedUserId.value);
    });

    const uniqueEvents = [...ownerEvents, ...participantEvents].reduce((acc, event) => {
      acc.set(event.eventId, event);
      return acc;
    }, new Map());

    uniqueEvents.forEach(x => {
      const eventDetails = {
        id: x.eventId,
        name: x.eventName,
        date: x.eventDate,
        note: x.eventNote,
        time: x.eventTime ? x.eventTime.slice(0, 5) : null,
        owner: x.ownerId,
        participantsIds: x.participantsIds || [] // Uložíme účastníky jako pole čísel
      };

      const key = new Date(x.eventDate).getTime();

      if (!events[key]) {
        events[key] = [];
      }

      events[key].push(eventDetails);
      sortEventsForDay(events[key]);
    });

  } catch (error) {
    console.log("Chyba při načítání událostí:", error);
  }
}

async function showEventDetail(event) {
  nameInp.value = null;
  dateInp.value = null;
  noteInp.value = null;
  timeInp.value = null;
  onEdit.value = false;
  participantsIncluded.value = null;
  selectedEvent.value = event;
  selectedEventParticipants.value = [];
  let selectedEventParticipantsIds = [];

  if (event.participantsIds) {
    try {
      selectedEventParticipantsIds = JSON.parse(event.participantsIds);
      participantsIncluded.value = true;
    } catch {
      selectedEventParticipantsIds = []; // Pokud je nevalidní JSON, nastavíme prázdné pole
      participantsIncluded.value = false;
    }
  }

  if (participantsIncluded.value === false) {
    eventDialog.value.showModal();
    return;
  }

  try {
    const response = await fetch(`https://localhost:5050/api/Users`);
    const data = await response.json();

    const uniqueParticipants = new Set();

    selectedEventParticipantsIds.forEach(x => {
      let user = data.find(y => y.userId === x);
      if (user) {
        uniqueParticipants.add(user.email);
      }
    });

    selectedEventParticipants.value = Array.from(uniqueParticipants);

    eventDialog.value.showModal();
  } catch (error) {
    console.error("Chyba při načítání účastníků:", error);
  }
}

async function startEdit(event) {
  participantsIds.value = null;
  onEdit.value = true;
  nameInp.value = selectedEvent.value.name;
  dateInp.value = selectedEvent.value.date.slice(0, 10);
  noteInp.value = selectedEvent.value.note ? selectedEvent.value.note : null;
  timeInp.value = selectedEvent.value.time ? selectedEvent.value.time : null;
  selectedEventParticipants.value = [];
  participants.value = [];
  participantsIds.value = [];
  let selectedEventParticipantsIds = [];

  if (event.participantsIds) {
    try {
      selectedEventParticipantsIds = JSON.parse(event.participantsIds);
      participantsIncluded.value = true;
      isHostOn.value = true;
    } catch {
      selectedEventParticipantsIds = []; // Pokud je nevalidní JSON, nastavíme prázdné pole
      participantsIncluded.value = false;
      isHostOn.value = false;
    }
  }
  try {
    const response = await fetch(`https://localhost:5050/api/Users`);
    const data = await response.json();

    const uniqueParticipants = new Set();

    selectedEventParticipantsIds.forEach(x => {
      let user = data.find(y => y.userId === x);
      if (user) {
        uniqueParticipants.add(user.email);
        participants.value.push(user);
        participantsIds.value.push(user.userId);
      }
    });
    participantsEmails.value = Array.from(uniqueParticipants);
  } catch (error) {
    console.error("Chyba při načítání účastníků:", error);
  }
}

async function saveEdit() {
  let updatedEvent;
  if (participantsIds.value) {
    updatedEvent = {
      eventId: selectedEvent.value.id,
      eventName: nameInp.value,
      eventDate: `${dateInp.value}T00:00:00.000Z`,
      eventTime: `${timeInp.value}:00`,
      eventNote: noteInp.value,
      ownerId: selectedEvent.value.owner,
      participantsIds: JSON.stringify(participantsIds.value)
    };
    await sendEmailUpdate(nameInp.value, dateInp.value, timeInp.value, noteInp.value, updatedEvent.ownerId);
  } else {
    updatedEvent = {
      eventId: selectedEvent.value.id,
      eventName: nameInp.value,
      eventDate: `${dateInp.value}T00:00:00.000Z`,
      eventTime: `${timeInp.value}:00`,
      eventNote: noteInp.value,
      ownerId: selectedEvent.value.owner,
      participantsIds: null
    };
  }
  try {
    await axios.post("https://localhost:5050/api/Events/UpdateEvent", updatedEvent);
    nameInp.value = null;
    dateInp.value = null;
    noteInp.value = null;
    timeInp.value = null;
    onEdit.value = false;
    eventDialog.value.close();

    await resetPage();
  } catch (error) {
    console.log(error);
  }
}

async function resetPage() {
  emit('onLoading');
  Object.keys(events).forEach(key => delete events[key]);
  await fetchEvents();
  sortAllEvents();
  drawCalendar();
  setTimeout(() => {
    emit('onLoading');
  }, 500)
}

</script>

<template>
  <div class="dialogContainer">
    <dialog ref="eventDialog" class="dialogs">
      <div v-if="selectedEvent" class="inputContainer">

        <div>
          <h3 v-if="!onEdit" class="dialogRow inputStyle" style="font-weight: bold">Událost:
            {{ selectedEvent.name }}</h3>

          <label v-if="onEdit">
            Název
            <input type="text" v-model="nameInp" class="dialogRow inputStyle" style="margin-bottom: 2%">
          </label>

          <p v-if="!onEdit" class="dialogRow inputStyle">Datum: {{
              new Date(selectedEvent.date).toLocaleDateString()
            }}</p>

          <label v-if="onEdit">
            Datum
            <input type="date" v-model="dateInp" class="dialogRow inputStyle" style="margin-bottom: 2%">
          </label>

          <p v-if="selectedEvent.time && !onEdit" class="dialogRow inputStyle">Čas: {{ selectedEvent.time }}</p>

          <label v-if="onEdit">
            Čas
            <input type="time" v-model="timeInp" class="dialogRow inputStyle" style="margin-bottom: 2%">
          </label>

          <p v-if="selectedEvent.note && !onEdit" class="dialogRow inputStyle">Poznámka: {{ selectedEvent.note }}</p>

          <label v-if="onEdit">
            Poznámka
            <input type="text" v-model="noteInp" class="dialogRow inputStyle" style="margin-bottom: 2%">
          </label>

          <h4 v-if="participantsIncluded && !onEdit">
            Účastníci
          </h4>
          <p v-for="participant in selectedEventParticipants" v-if="!onEdit">{{ participant }}</p>

          <button @click="isHostOn = true" v-if="!isHostOn && onEdit" class="dialogRow">Přidat hosty</button>

          <div v-if="isHostOn && onEdit" class="dialogColum">
            Účastníci
            <div class="dialogRow">
              <input type="email" v-model="currentParticipant" class="inputStyle" placeholder="Host"
                     style="border-right: none">
              <button @click="addParticipant()" id="buttonInvite"
                      style="border-bottom-left-radius: 0; border-top-left-radius: 0; border-left: none;">Pozvat
              </button>
            </div>
            <div v-for="parti in participantsEmails">
              {{ parti }}
            </div>
          </div>
        </div>
        <div class="dialogRow" style="gap: 10px">
          <button @click="startEdit(selectedEvent)" v-if="!onEdit" class="editBtn">Upravit</button>
          <button @click="eventDialog.close()" v-if="!onEdit">Zavřít</button>
        </div>
        <button class="dialogRow addBtn" @click="saveEdit()" v-if="onEdit" style="margin-top: -5%">Uložit</button>
      </div>
    </dialog>
  </div>
  <div class="dialogContainer">
    <dialog ref="addEventDialog" class="dialogs">
      <div class="inputContainer">
        <input class="inputStyle dialogRow" type="text" v-model="eventName" placeholder="Název události">

        <div class="dialogRow">
          <input class="inputStyle" type="date" v-model="selectedDate" style="border-right: none">
          <button @click="isTimeOn = true" v-if="!isTimeOn" id="buttonTime"
                  style="border-bottom-left-radius: 0; border-top-left-radius: 0; border-left: none;">Přidat čas
          </button>
          <input class="inputStyle" id="inputTime" type="time" step="900" v-model="eventTime" v-if="isTimeOn"
                 style="border-left: none">
        </div>

        <button @click="isHostOn = true" v-if="!isHostOn" class="dialogRow">Přidat hosty</button>

        <div v-if="isHostOn" class="dialogColumn">
          <div class="dialogRow">
            <input type="email" v-model="currentParticipant" class="inputStyle" placeholder="Host"
                   style="border-right: none">
            <button @click="addParticipant()" id="buttonInvite"
                    style="border-bottom-left-radius: 0; border-top-left-radius: 0; border-left: none;">Pozvat
            </button>
          </div>
          <div v-for="parti in participantsEmails" class="dialogRow" style="font-size: smaller">
            {{ parti }}
          </div>
        </div>

        <button @click="isNoteOn = true" v-if="!isNoteOn" class="dialogRow">Přidat poznámku</button>
        <input type="text" v-model="eventNote" v-if="isNoteOn" class="inputStyle dialogRow" placeholder="Poznámka">
        <button @click="addEvent()" class="dialogRow addBtn">Přidat</button>
      </div>
    </dialog>
  </div>
  <body>

  <header>
    <p>Po</p>
    <p>Út</p>
    <p>St</p>
    <p>Čt</p>
    <p>Pá</p>
    <p>So</p>
    <p>Ne</p>
  </header>

  <main>
    <Day
        v-for="day in daysBefore"
        :key="day"
        :day="day.getDate()"
        :filteredEvents="filteredEvents[day.getTime()] || []"
        @addEvent="showModal(day)"
        @removeEvent="(index) => removeEvent(day, index)"
        :class="{ 'notCurrentMonth': true, 'today': isToday(day) }"

    />

    <Day
        v-for="day in daysCurrent"
        :key="day"
        :day="day.getDate()"
        :events="filteredEvents[day.getTime()] || []"
        @addEvent="showModal(day)"
        @removeEvent="(index) => removeEvent(day, index)"
        :class="{ 'today': isToday(day) }"
        :name="filteredHolidays.find(holiday => holiday.id && new Date(holiday.id).getDate() === day.getDate())?.name || ''"
        @showEvent="showEventDetail"
    />

    <Day
        v-for="day in daysAfter"
        :key="day"
        :day="day.getDate()"
        :events="filteredEvents[day.getTime()] || []"
        @addEvent="showModal(day)"
        @removeEvent="(index) => removeEvent(day, index)"
        :class="{ 'notCurrentMonth': true, 'today': isToday(day) }"

    />

  </main>
  </body>
</template>

<style scoped>

header {
  display: flex;
  flex-direction: row;
  gap: 13.75%;
  margin-left: 7%;
  margin-bottom: 1%;
  color: grey;
}

body {
  display: flex;
  flex-direction: column;
  margin-left: 10%;
  margin-top: 1%;
  background-color: #181818;
}

main {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
}

button {
  background-color: #36363650;
  border: 1px solid #36363690;
  color: White;
  cursor: pointer;
  text-align: center;
  padding: 5px 10px;
  font-size: 16px;
  height: 30px;
  border-radius: 5px;
}

.editBtn {
  border: 1px solid cornflowerblue;
  box-shadow: inset 0 0 0 1px cornflowerblue;
  background: linear-gradient(to right, #36363620, #36363600);
  transition: box-shadow 0.3s ease-in-out;
}

.editBtn:hover {
  box-shadow: inset 0 0 2px 3px cornflowerblue;
}

span {
  font-size: 18px;
}

.notCurrentMonth {
  color: darkgrey;
}

.today {
  border: solid 2px cornflowerblue; /* světle žlutá nebo jiná barva pro zvýraznění */
  color: white;
  font-weight: bold;
}

main {
  background-color: #1f1f1f;
  padding: 10px;
  border-radius: 10px;
}

.dialogs {
  width: 400px;
  padding: 20px;
  border: none;
  border-radius: 8px;
  flex-direction: column;
  gap: 10px;
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background-color: #181818;
  color: white;
}

.inputContainer {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.dialogRow {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  height: 35px;
}

.editDialogRow {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  height: 25px;
  margin-bottom: 2%;
}

.dialogRow:focus,
.dialogRow input:focus {
  outline: none;
  background: linear-gradient(to top, #36363620, #36363600);
}

.dialogRow input,
.dialogRow button,
#host button {
  flex: 3;
  height: 35px;
}

.addBtn {
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
}

.addBtn:hover {
  background-position: right;
}

.inputStyle {
  width: 100%;
  padding: 8px;
  background-color: transparent;
  border: none;
  border-bottom: 1px solid #363636;
  color: white;
}

#buttonTime, #buttonInvite, #inputTime {
  flex: 1;
}

.dialogColumn {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

input[type="date"]::-webkit-calendar-picker-indicator {
  opacity: 0; /* Skryje ikonku */
  position: absolute;
  width: 60%;
  cursor: pointer;
}

input[type="time"]::-webkit-calendar-picker-indicator {
  opacity: 0; /* Skryje ikonku */
  position: absolute;
  width: 20%;
  cursor: pointer;
}

input[type="date"], input[type="time"] {
  color-scheme: dark;
}
</style>
