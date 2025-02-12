<script setup>
import Day from "@/components/day.vue";
import {onMounted, reactive, ref, watch} from "vue";
import axios from "axios";

const apiUrl = 'https://172.20.10.4:5050';

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
const allDayEvent = ref(null);

const currentParticipant = ref(null);
const participants = ref(null);
const participantsIds = ref(null);
const participantsEmails = ref(null);
const participantsIncluded = ref(null);

const onEdit = ref(false);

const nationalHolidays = ref([]);

const today = new Date();

const props = defineProps(['loggedUser', 'action', 'loggedUsername']);
const isToday = (day) => {
  return day.getDate() === today.getDate() &&
      day.getMonth() === today.getMonth() &&
      day.getFullYear() === today.getFullYear();
}

onMounted(async () => {
  const date = new Date();
  currentMonth.value = date.getMonth();
  currentYear.value = date.getFullYear();
  daysInCurrentMonth.value = new Date(currentYear.value, currentMonth.value + 1, 0).getDate();

  await fetchEvents();

  sortAllEvents();
  await fetchHolidays();
  drawCalendar();
});

watch(events, () => {
  localStorage.setItem('events', JSON.stringify(events));
  sortAllEvents();
}, {deep: true});

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
  console.log(selectedDay.value);
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
        await axios.post(`${apiUrl}/api/Mail`, emailDetails);
        console.log(`Email odeslán na ${x.email}`);
      } catch (error) {
        console.error(`Chyba při odesílání e-mailu na ${x.email}:`, error);
      }
  }
}

async function addEvent() {
  let newEvent;
  const key = selectedDay.value.getTime();

  if (!eventName.value) {
    alert('Zadejte název události!');
    return;
  }
  if (!eventTime.value && !allDayEvent.value) {
    alert('Zadejte čas nebo vyberte "Celý den"!');
    return;
  }

  if (!events[key]) {
    events[key] = [];
  }

  const eventDetails = {
    name: eventName.value,
  };

  if (!allDayEvent.value && eventTime.value) {
    eventDetails.time = eventTime.value;
  }

  events[key].push(eventDetails);
  sortEventsForDay(events[key]);

  if (!Array.isArray(participantsIds.value)) {
    participantsIds.value = [];
  }

  console.log(eventTime.value);
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
  allDayEvent.value = false;
  participantsIds.value = null;

  addEventDialog.value.close();

  await axios.post(`${apiUrl}/api/Events`, newEvent);
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

    const response = await fetch(`${apiUrl}/api/Users`);
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
  await axios.delete(`${apiUrl}/api/Events/${index}`);
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
  const url = `${apiUrl}/api/Events`;

  try {
    const response = await fetch(url);
    const data = await response.json();
    const ownerEvents = data.filter(x => x.ownerId === props.loggedUser);

    const participantEvents = data.filter(x => {
      if (!x.participantsIds) return false;
      return x.participantsIds.includes(props.loggedUser);
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
  participantsIncluded.value = null;
  selectedEvent.value = event;
  selectedEventParticipants.value = [];

  let selectedEventParticipantsIds = [];

  // Kontrola, zda event.participantsIds existuje a není prázdný
  if (event.participantsIds) {
    try {
      selectedEventParticipantsIds = JSON.parse(event.participantsIds);
      participantsIncluded.value = true;
    } catch {
      selectedEventParticipantsIds = []; // Pokud je nevalidní JSON, nastavíme prázdné pole
      participantsIncluded.value = false;
    }
  }

  // Pokud nejsou žádní účastníci, neprovádíme další kroky
  if (participantsIncluded.value === false) {
    eventDialog.value.showModal();
    return;
  }

  try {
    const response = await fetch(`${apiUrl}/api/Users`);
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

watch(props.action, () => {
  console.log(props.action)
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

function startEdit(){
  onEdit.value = true;
}
function saveEdit(){
  onEdit.value = false;
}
</script>

<template>
  <div class="dialogContainer">
    <dialog ref="eventDialog" class="dialogs">
      <div v-if="selectedEvent">
        <a @click.prevent="startEdit" v-if="!onEdit">Upravit</a>

        <h3 v-if="!onEdit">Událost: {{ selectedEvent.name }}</h3>
        <input type="text" v-if="onEdit" v-model="nameInp">

        <div>
          <p v-if="!onEdit">Datum: {{ new Date(selectedEvent.date).toLocaleDateString() }}</p>
          <input type="date" v-if="onEdit" v-model="dateInp">

          <p v-if="selectedEvent.time && !onEdit">Čas: {{ selectedEvent.time }}</p>
          <input type="time" v-if="onEdit" v-model="timeInp">

          <p v-if="selectedEvent.note && !onEdit">Poznámka: {{selectedEvent.note}}</p>
          <input type="text" v-if="onEdit">

          <h4 v-if="participantsIncluded && !onEdit">
            Účastníci
          </h4>
          <p v-for="participant in selectedEventParticipants" v-if="!onEdit">{{ participant }}</p>

          <button @click="isHostOn = true" v-if="!isHostOn && onEdit">Přidat hosty</button>

          <div v-if="isHostOn && onEdit" id="host">
            <input type="email" v-model="currentParticipant" class="inputStyle" placeholder="Host">
            <button @click="addParticipant()" class="tlacitko">Pozvat</button>
            <div v-for="parti in participantsEmails">
              {{ parti }}
            </div>
          </div>
        </div>
        <a @click.prevent="saveEdit()" v-if="onEdit">Uložit</a>
        <button @click="eventDialog.close()" v-if="!onEdit">Zavřít</button>
      </div>
    </dialog>
  </div>
  <div class="dialogContainer">
    <dialog ref="addEventDialog" class="dialogs">
      <div>
        <div class="inputContainer">
          <input class="inputStyle" id="inputName" type="text" v-model="eventName" placeholder="Název události">

          <div id="dateAndTime">
            <input class="inputStyle" id="inputDate" type="date" v-model="selectedDate">
            <button @click="isTimeOn = true" v-if="!isTimeOn" class="tlacitko">Přidat čas</button>
            <input class="inputStyle" id="inputTime" type="time" v-model="eventTime" v-if="isTimeOn">
          </div>

          <button @click="isHostOn = true" v-if="!isHostOn">Přidat hosty</button>

          <div v-if="isHostOn" id="host">
            <input type="email" v-model="currentParticipant" class="inputStyle" placeholder="Host">
            <button @click="addParticipant()" class="tlacitko">Pozvat</button>
            <div v-for="parti in participantsEmails">
              {{ parti }}
            </div>
          </div>

          <button @click="isNoteOn = true" v-if="!isNoteOn" class="tlacitko">Přidat poznámku</button>
          <input type="text" v-model="eventNote" v-if="isNoteOn" class="inputStyle" placeholder="Poznámka">
        </div>
      </div>
      <div>
        <label id="wholeDay">
          Celý den
          <input type="checkbox" v-model="allDayEvent">
        </label>
      </div>
      <button @click="addEvent()">Přidat</button>
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
    <!--    dny predchoziho mesice-->
    <Day
        v-for="day in daysBefore"
        :key="day"
        :day="day.getDate()"
        :events="events[day.getTime()] || []"
        @addEvent="showModal(day)"
        @removeEvent="(index) => removeEvent(day, index)"
        :class="{ 'notCurrentMonth': true, 'today': isToday(day) }"

    />

    <!--    dny aktualniho mesice-->
    <Day
        v-for="day in daysCurrent"
        :key="day"
        :day="day.getDate()"
        :events="events[day.getTime()] || []"
        @addEvent="showModal(day)"
        @removeEvent="(index) => removeEvent(day, index)"
        :class="{ 'today': isToday(day) }"
        :name="nationalHolidays.find(holiday => holiday.id && new Date(holiday.id).getDate() === day.getDate())?.name || ''"
        @showEvent="showEventDetail"
    />

    <!--    dny nasledujici mesic-->
    <Day
        v-for="day in daysAfter"
        :key="day"
        :day="day.getDate()"
        :events="events[day.getTime()] || []"
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
}

body {
  display: flex;
  flex-direction: column;
  margin-left: 10%;
  margin-top: 1%;

}

h1 {
  margin-bottom: 1%;
}

main {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
}

.calendar-controls {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 20px;
}

button {
  margin: 0 10px;
  padding: 5px 10px;
  font-size: 16px;
  cursor: pointer;
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
  border: none;
  background-color: #1f1f1f;
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
}

.inputStyle {
  background-color: #1f1f1f;
  border: 1px solid #36363690;
  height: 30px;
  width: 300px;
}

#wholeDay {
  display: flex;
  gap: 4px;
  align-items: center;
  color: white;
}

.inputContainer {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

#inputName, #inputDate, #inputTime {
  color: white;
}

#inputName:focus, #inputTime:focus, #inputDate:focus {
  outline: none;
}

#dateAndTime, #host {
  width: 300px;
  display: flex;
}

.tlacitko {
  height: 30px;
}
</style>
