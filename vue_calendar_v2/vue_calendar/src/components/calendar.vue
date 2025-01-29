<script setup>
import Day from "@/components/day.vue";
import { onMounted, reactive, ref, watch } from "vue";
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

const selectedEvent = ref(null);
const eventDialog = ref(null);

const eventName = ref(null);
const eventTime = ref(null);
const eventNote = ref(null);
const allDayEvent = ref(null);

const nationalHolidays = ref([]);

const today = new Date();
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
}, { deep: true });

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
    daysAfter.value.push(new Date(sussyYear, nextMonth, z +1));
  }
  fetchHolidays()
}
function showModal(day){
  addEventDialog.value.showModal();
  selectedDay.value = day;
}
async function addEvent() {
  const key = selectedDay.value.getTime();

  if (!eventName.value){
    alert('Zadejte název události!');
    return;
  }
  if (!eventTime.value && !allDayEvent.value){
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

  let newEvent;

  if(!eventTime.value){
    newEvent = {
      eventName: eventName.value,
      eventDate: `${selectedDay.value.getFullYear()}-${(selectedDay.value.getMonth() + 1).toString().padStart(2, '0')}-${selectedDay.value.getDate().toString().padStart(2, '0')}`,
      eventTime: null,
      eventNote: eventNote.value,
      ownerId: 1
    };
  }else{
    newEvent = {
      eventName: eventName.value,
      eventDate: `${selectedDay.value.getFullYear()}-${(selectedDay.value.getMonth() + 1).toString().padStart(2, '0')}-${selectedDay.value.getDate().toString().padStart(2, '0')}`,
      eventTime: `${eventTime.value}:00`,
      eventNote: eventNote.value,
      ownerId: 1
    };
  }

  eventName.value = null;
  eventTime.value = null;
  eventNote.value = null;
  allDayEvent.value = false;

  addEventDialog.value.close();

  await axios.post("https://localhost:7198/api/Events", newEvent);
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

//prebira dany den a event ktery chceme smazat
async function removeEvent(day, index) {

  events[new Date(day).getTime()].splice(index, 1);
  await axios.delete(`https://localhost:7198/api/Events/${index}`);
}

//porovnavani casu u eventu ve dni
//bere to vzdy 2 prvky a ty porovnava podle podminek
//1. pokud A nema cas (celodenni event) a B cas ma, tak A pujde pred B
//2. pokud A ma cas a B cas nema (celodenni event), tak B jde pred A
//3. pokud oba maji cas, tak to pomoci funkce localeCompare porovna, ktery cas je drive a nasledne srovna
//return pokud ani jeden cas nema -> neni co resit
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

async function fetchHolidays(){
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

async function fetchEvents(){
  const url = "https://localhost:7198/api/Events";

  try {
    const response = await fetch(url);
    const data = await response.json();

    data.forEach(x => {
      const eventDetails = {
        id: x.eventId,
        name: x.eventName,
        date: x.eventDate,
        note: x.eventNote,
        time: x.eventTime,
        owner: x.ownerId,
        participant: x.participantId
      };

      if (!allDayEvent.value && eventTime.value) {
        eventDetails.time = eventTime.value;
      }

      const key = new Date(x.eventDate).getTime();

      if (!events[key]) {
        events[key] = [];
      }

      events[key].push(eventDetails);
      sortEventsForDay(events[key]);
    })

  } catch (error) {
    console.log(error);
  }

}

function showEventDetail(event){
  selectedEvent.value = event;
  eventDialog.value.showModal();
}
</script>

<template>
  <dialog ref="eventDialog">
    <div v-if="selectedEvent">
      <h3>Událost: {{selectedEvent.name}}</h3>
      <div>
      <p v-if="selectedEvent.date">Datum: {{new Date(selectedEvent.date).toLocaleDateString()}}</p>
      <p v-if="selectedEvent.time">Čas: {{selectedEvent.time}}</p>
      </div>
    </div>
  </dialog>
  <dialog ref="addEventDialog">
    <div>
      <input type="text" v-model="eventName" placeholder="Název události">
      <input type="time" v-model="eventTime" v-if="!allDayEvent">
      <label>
        Celý den
        <input type="checkbox" v-model="allDayEvent">
      </label>

    </div>
    <div>
      <input type="text" placeholder="Poznámka" v-model="eventNote">
    </div>
    <button @click="addEvent()">Přidat</button>
  </dialog>

  <body>
  <div class="calendar-controls">
    <button @click="previousMonth">Předchozí</button>
    <button @click="nextMonth">Následující</button>
    <button @click="showNow">Nyní</button>
  </div>

  <!--  zobrazeni aktualniho mesice a roku-->
  <h1>{{new Date(currentMonth, currentMonth).toLocaleDateString('cs-CZ', { month: 'long' })}} {{currentYear}}</h1>

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
        :name="nationalHolidays.find(holiday => new Date(holiday.id).getDate() === day.getDate()).name"
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
body{
  display: flex;
  flex-direction: column;
  margin-left: 10%;
  margin-top: 5%;
}

h1{
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

.notCurrentMonth{
  color: darkgrey;
}

.today {
  border: solid 2px cornflowerblue; /* světle žlutá nebo jiná barva pro zvýraznění */
  color: white;
  font-weight: bold;
}
</style>
