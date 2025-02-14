<script setup>
import {onMounted, ref, watch} from "vue";
import LittleCalendar from "@/components/littleCalendar.vue";

const props = defineProps(['loggedUser', 'currentMonth', 'currentYear', 'createEvent']);
const emit = defineEmits(['logOut', 'userEvents', 'holidayEvents']);
const hideEvents = ref(false);
const hideHolidays = ref(false);
const daysCurrent = ref([]);
const daysInCurrentMonth = ref(0);

onMounted(() => {
  drawCalendar();
})
watch([() => props.currentMonth, () => props.currentYear], () => {
  drawCalendar();
});
function drawCalendar() {
  console.log('start');
  daysCurrent.value = [];
  daysInCurrentMonth.value = new Date(props.currentYear.value, props.currentMonth.value + 1, 0).getDate();

  let currentDay = 1;
  for (let i = 0; i < daysInCurrentMonth.value; i++) {
    console.log(daysInCurrentMonth.value)
    console.log(i);
    daysCurrent.value.push(i);
    currentDay++;
  }
}
function logOut() {
  emit('logOut');
  window.location.reload();
}
function toggleUserEvents() {
  hideEvents.value = !hideEvents.value
  emit('userEvents', hideEvents.value)
}
function toggleHolidayEvents() {
  hideHolidays.value = !hideHolidays.value
  emit('holidayEvents', hideHolidays.value)
}
function createEvent()
{
  props.createEvent.action = {};
  props.createEvent.action = true;
}
</script>

<template>

  <section>
    <div class="row">
      <a class="material-symbols-outlined"> person </a>
      <p>{{ props.loggedUser }}</p>
    </div>
    <div class="row">
      <button class="tlacitko" @click="createEvent">Vytvořit událost</button>
    </div>
    <div class="row">
      <div id="smolCalendar">
        <littleCalendar v-for="day in daysCurrent"
                        :key="day"
                        :day="day"></littleCalendar>
      </div>
    </div>
    <div class="row">
      <div id="checkboxContainer">
        <h3>Moje kalendáře</h3>
        <label>
          <input type="checkbox" name="" id="loggedUserCheck" @change="toggleUserEvents" :checked="!hideEvents">
          {{ props.loggedUser }}
        </label>
        <label>
          <input type="checkbox" name="" id="holidayCheck" @change="toggleHolidayEvents" :checked="!hideHolidays">
          Svátky
        </label>
      </div>
    </div>
    <div class="row">

    </div>
    <div id="lastRow">
      <button class="tlacitko" @click="logOut">Log out</button>
    </div>
    <div id="logoBtm">
      <img src="">
    </div>
  </section>
</template>

<style scoped>
section {
  display: flex;
  flex-direction: column;
  background: #1f1f1f;
  width: 14vw;
  height: 70vh;
}

.row, #lastRow {
  display: flex;
  align-items: center;
  width: 90%;
  color: white;
  margin-left: 5%;
  margin-right: 5%;
}

#checkboxContainer {
  display: flex;
  flex-direction: column;
}

.tlacitko {
  background-color: #36363650;
  border: 1px solid #36363690;
  padding: 5%;
  color: White;
  border-radius: 5px;
}

label {
  display: flex;
  gap: 3px;
}

button {
  width: 100%;
}

#smolCalendar {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
}
</style>