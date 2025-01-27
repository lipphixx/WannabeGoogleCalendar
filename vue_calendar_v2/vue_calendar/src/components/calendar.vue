<script setup>
import Day from "@/components/day.vue";
import { onMounted, reactive, ref, watch } from "vue";

const currentYear = ref(0);
const currentMonth = ref(0);
const daysInCurrentMonth = ref(0);

const daysBefore = ref([]);
const daysCurrent = ref([]);
const daysAfter = ref([]);

//pouzivam reactive, protoze je to slozitejsi datovy typ
const events = reactive({});

const addEventDialog = ref(null);
const selectedDay = ref(null);

const eventName = ref(null);
const eventTime = ref(null);
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

  // pokud eventy existuji, vytvori se do pole, pokud ne, vytvori se prazdne pole
  const loadedEvents = JSON.parse(localStorage.getItem('events')) || {};

  for (let key in loadedEvents) {
    events[key] = loadedEvents[key]; //nacitam ulozene eventy daneho dne co pole vsech eventu
  }

  sortAllEvents(); //serazeni
  await fetchHolidays();
  drawCalendar();
});

watch(events, () => {
  localStorage.setItem('events', JSON.stringify(events));
  sortAllEvents();
}, { deep: true });
//deep: true sleduje hlubsi zmeny nez v prvni vrstve - pokud se neco zmeni v day, tak i tak to bude sledovat

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
function addEvent() {
  const key = selectedDay.value.getTime();
  if (!eventName.value){
    alert('Zadejte název události!');
    return;
  }
  if (!eventTime.value && !allDayEvent.value){
    alert('Zadejte čas nebo vyberte "Celý den"!');
    return;
  }

  //pokud na danem dni neni zadny event, vytvori se tam prazdne pole
  if (!events[key]) {
    events[key] = [];
  }

  const eventDetails = {
    name: eventName.value,
  };

  //defaultne se prida jen nazev eventu, pokud je zadan i cas, tak se zde prida i cas eventu
  if (!allDayEvent.value && eventTime.value) {
    eventDetails.time = eventTime.value;
  }

  //na danem dni se prida event
  events[key].push(eventDetails);
  sortEventsForDay(events[key]);

  //reset dialogu
  eventName.value = null;
  eventTime.value = null;
  allDayEvent.value = false;

  addEventDialog.value.close();
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
function removeEvent(day, index) {
  const key = day.getTime();
  events[key].splice(index, 1);
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
  console.log(fullUrl);

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
</script>

<template>
  <dialog ref="addEventDialog">
    <input type="text" v-model="eventName">
    <input type="time" v-model="eventTime" v-if="!allDayEvent">
    <label>
      Celý den
      <input type="checkbox" v-model="allDayEvent">
    </label>
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
    />

    <!--    dny nasledujici mesic-->
    <Day
        v-for="day in daysAfter"
        :key="day"
        :day="day.getDate()"
        :events="events[day.getTime()] || []"
        @addEvent="addEvent(day)"
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
