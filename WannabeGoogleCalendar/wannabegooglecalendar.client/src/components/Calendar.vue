<script setup>
import {onMounted, reactive, ref} from "vue";
import Day from "@/components/Day.vue";

const month = ref(0);
const year = ref(0);
const days = ref(0);
const daysArray = ref([]);
const daysBefore = ref([]);
const daysAfter = ref([]);

const eventName = ref(null);
const eventTime = ref(0);
const eventDialog = ref(null);
const eventsArray = reactive({});
const selectedDay = ref(null);
const isAllDay = ref(false);

onMounted(() => {
  let dateNow = new Date()
  month.value = dateNow.getMonth();
  year.value = dateNow.getFullYear();
  loadPage();
  drawPage();
});

function savePage() {
  localStorage.setItem("events", JSON.stringify(eventsArray));
}

function loadPage() {
  const savedEvents = localStorage.getItem("events");
  if (savedEvents) {
    if (savedEvents) {
      const eventsJson = JSON.parse(savedEvents);
      for (let key in eventsJson) {
        eventsArray[key] = eventsJson[key];
      }
    }
  }
}

function drawPage() {
  days.value = new Date(year.value, month.value + 1, 0).getDate();
  let firstDay = new Date(year.value, month.value, 0).getDay();
  let currentDay = 1;

  let prevMonth = month.value - 1;
  let prevYear = year.value;
  if (prevMonth < 0) {
    prevMonth = 11;
    prevYear--;
  }

  let daysInPreviousMonth = new Date(prevYear, month.value, 0).getDate();
  daysBefore.value = [];
  daysArray.value = [];
  daysAfter.value = [];

  for (let x = daysInPreviousMonth - firstDay; x < daysInPreviousMonth; x++) {
    daysBefore.value.push(new Date(prevYear, prevMonth, x + 1));
  }

  for (let y = 0; y < days.value; y++) {
    daysArray.value.push(new Date(year.value, month.value, currentDay));
    currentDay++;
  }

  let nextMonth = month.value + 1;
  let nextYear = year.value;
  if (nextMonth > 11) {
    nextMonth = 0;
    nextYear++;
  }

  let seven = 7 - new Date(year.value, month.value + 1, 0).getDay();
  for (let z = 0; z < seven; z++) {
    daysAfter.value.push(new Date(nextYear, nextMonth, z + 1));
  }
}

function nextMonth() {
  if (month.value === 11) {
    month.value = 0;
    year.value++;
  } else {
    month.value++;
  }
  drawPage();
}

function previousMonth() {
  if (month.value === 0) {
    month.value = 11;
    year.value--;
  } else {
    month.value--;
  }
  drawPage();
}

function nowMonth() {
  let dateNow = new Date()
  month.value = dateNow.getMonth();
  year.value = dateNow.getFullYear();
  drawPage();
}

function dialogEvent(day) {
  eventDialog.value.showModal();
  selectedDay.value = day;
}

function saveEvent(eventNamea, eventTimea) {
  const key = selectedDay.value.getTime();
  if (!eventName) return;

  eventName.value = '';
  eventTime.value = '';

  if (eventTimea === '') {
    isAllDay.value = true;
  }

  if (!eventsArray[key]) {
    eventsArray[key] = [];
  }

  eventsArray[key].push({name: eventNamea, time: isAllDay.value ? null : eventTimea});
  isAllDay.value = false;

  eventsArray[key].sort((a, b) => {
    if (a.time === null && b.time !== null) return -1;
    if (a.time !== null && b.time === null) return 1;
    if (a.time === null && b.time === null) return 0;
    const timeA = new Date(`1970-01-01T${a.time}:00`);
    const timeB = new Date(`1970-01-01T${b.time}:00`);
    return timeA - timeB;
  });

  eventDialog.value.close();
  savePage();
}

function deleteEvent(day, index) {
  const key = day.getTime();
  eventsArray[key].splice(index, 1);
  if (eventsArray[key].length === 0) {
    delete eventsArray[key];
  }
  savePage();
}

function closeDialog() {
  eventDialog.value.close();
}

</script>

<template>
  <dialog ref="eventDialog" id="dialogicek">
    <div id="divInDialog">
      <label>
        Název události:
        <input type="text" v-model="eventName">
      </label>
      <label>
        Datum a čas události: (nepovinné)
        <input type="time" name="" id="" v-model="eventTime">
      </label>
      <div>
        <button @click="saveEvent(eventName, eventTime)">Přidat</button>
        <button @click="closeDialog">Zavřít</button>
      </div>
    </div>
  </dialog>
  <div id="container">
    <section id="buttons">
      <button @click="previousMonth">Dříve</button>
      <button @click="nextMonth">Později</button>
      <button @click="nowMonth">NYNÍ</button>
      <h1>{{ new Date(year, month).toLocaleDateString('cs-CZ', {month: 'long'}) }} {{ year }}</h1>
    </section>
    <section id="calendar">
      <header>
        <div>po</div>
        <div>út</div>
        <div>st</div>
        <div>čt</div>
        <div>pá</div>
        <div>so</div>
        <div>ne</div>
      </header>
      <div id="meow">
        <Day v-for="day in daysBefore"
             :key="day"
             :day="day.getDate()"
             :events="eventsArray[day.getTime()] || []"
             @click="dialogEvent(day)"
             @removeEvent="deleteEvent(day, $event)"
             class="daysAround">
        </Day>
        <Day v-for="day in daysArray"
             :key="day"
             :day="day.getDate()"
             :isToday="day.toDateString() === new Date().toDateString()"
             :events="eventsArray[day.getTime()] || []"
             @click="dialogEvent(day)"
             @removeEvent="deleteEvent(day, $event)">
        </Day>
        <Day v-for="day in daysAfter"
             :key="day"
             :day="day.getDate()"
             :events="eventsArray[day.getTime()] || []"
             @click="dialogEvent(day)"
             @removeEvent="deleteEvent(day, $event)"
             class="daysAround">
        </Day>
      </div>
    </section>
  </div>
</template>

<style scoped>
header div {
  border: 1px solid black;
  width: 150px;
}

header {
  display: flex;
}

div#meow {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
}

#buttons {
  display: flex;
  gap: 5px;
  margin-bottom: 5px;
}

#buttons button {
  width: 8%;
  background: #E0E1DD90;
  border: 2px solid #00000020;
  border-radius: 5px;
}

section#calendar {
  display: flex;
  flex-direction: column;
}

div#divInDialog {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 5px;
}

.daysAround {
  background: #00000020;
}

div#container {
  display: flex;
  flex-direction: column;
}

dialog#dialogicek {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: white;
  border-radius: 10px;
  border: 2px solid #00000020;
}

#divInDialog div {
  display: flex;
  justify-content: space-between;
}

#divInDialog div button {
  width: 40%;
  background: #E0E1DD90;
  border: 2px solid #00000020;
  border-radius: 5px;
}
</style>