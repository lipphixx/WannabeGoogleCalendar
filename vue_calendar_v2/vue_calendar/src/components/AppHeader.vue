<script setup>

const props = defineProps(['action', 'currentMonth', 'currentYear']);
const emit = defineEmits(['openMenu'])

function showMenuDialog()
{
  emit('openMenu');
}

function nextMonth() {
  props.action.action = 0;
  props.action.action = 2;
  if (currentMonth.value === 11) {
    currentMonth.value = 0;
    currentYear.value++;
  } else {
    currentMonth.value++;
  }
}
function previousMonth() {
  props.action.action = 0;
  props.action.action = 1;
  if (props.currentMonth.value === 0) {
    props.currentMonth.value = 11;
    props.currentYear.value--;
  } else {
    props.currentMonth.value--;
  }
}
function showNow() {
  props.action.action = 0;
  props.action.action = 3;
  const now = new Date();
  props.currentMonth.value = now.getMonth();
  props.currentYear.value = now.getFullYear();
}
</script>

<template>
  <head>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0"/>
  </head>

  <header>
    <div class="tlacitka">
      <button class="tlacitko" @click="previousMonth">Předchozí</button>
      <button class="tlacitko" @click="nextMonth">Následující</button>
      <button class="tlacitko" @click="showNow">Nyní</button>
    </div>

    <h1>{{new Date(props.currentMonth, props.currentMonth).toLocaleDateString('cs-CZ', { month: 'long' })}} {{props.currentYear}}</h1>

    <a class="material-symbols-outlined" @click.prevent="showMenuDialog"> menu </a>

  </header>
</template>

<style scoped>
header {
  height: 50px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #1f1f1f;
  width: 100%;
  padding-left: 2%;
  padding-right: 2%;
}
h1 {
  margin-right: 11%;
  text-align: center;
  color: rgba(255, 255, 255, 0.9);
}
#firRow {
  display: flex;
  align-items: center;
  margin-left: -5px;
}
.tlacitka {
  display: flex;
  gap: 5px;
}
.tlacitko {
  background-color: #36363650;
  border: 1px solid #36363690;
  padding: 5%;
  color: White;
  border-radius: 5px;
}
p {
  color: white;
}
.tlacitko:hover {
  background-color: #36363680;
  -webkit-box-shadow:inset 0px 0px 0px 3px rgba(81, 81, 81, 0.30);
}
#popupMenu {
  margin-left: auto;
  margin-right: 0;
  margin-top: 3%;
  width: 10%;
  height: 15%;
  border: none;
  background-color: #1f1f1f;
}
#popupMenu::backdrop {
  background: none;
}
</style>