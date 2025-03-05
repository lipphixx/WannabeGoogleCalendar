<script setup>

import {computed, onMounted, ref} from "vue";

const props = defineProps(['action', 'currentMonth', 'currentYear']);
const emit = defineEmits(['openMenu'])
const localMonth = ref(props.currentMonth);
const localYear = ref(props.currentYear);

const dateNow = computed(() => {
  return new Date(localYear.value, localMonth.value).toLocaleDateString("cs-CZ", { month: "long", year: "numeric" });
});

onMounted(() => {
  localMonth.value = props.currentMonth;
  localYear.value = props.currentYear;
});

function showMenuDialog()
{
  emit('openMenu');
}

function nextMonth() {
  props.action.action = 0;
  props.action.action = 2;
  if (props.currentMonth.value === 11) {
    localMonth.value = 0;
    localYear.value++;
  } else {
    localMonth.value++;
  }
}
function previousMonth() {
  props.action.action = 0;
  props.action.action = 1;
  if (props.currentMonth.value === 0) {
    localMonth.value = 11;
    localYear.value--;
  } else {
    localMonth.value--;
  }
}
function showNow() {
  props.action.action = 0;
  props.action.action = 3;
  const now = new Date();
  localMonth.value = now.getMonth();
  localYear.value = now.getFullYear();
}
</script>

<template>
  <head>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0"/>
  </head>

  <header>
    <div class="tlacitka">
      <button @click="previousMonth">Předchozí</button>
      <button @click="nextMonth">Následující</button>
      <button @click="showNow">Nyní</button>
    </div>

    <h1>{{dateNow}}</h1>

    <a class="material-symbols-outlined menu" @click.prevent="showMenuDialog"> menu </a>

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

.tlacitka {
  display: flex;
  gap: 5px;
}

button {
  background-color: #36363650;
  border: 2px solid #36363690;
  box-shadow: inset 0 0 0 0 cornflowerblue;
  padding: 5%;
  color: White;
  border-radius: 5px;
  transition: border 0.3s ease-in-out, box-shadow 0.3s ease-in-out;
  cursor: pointer;
}
p {
  color: white;
}

button:hover {
  border: 2px solid cornflowerblue;
  box-shadow: inset 0 0 0 1px cornflowerblue;
}
.menu  {
  transition: color 0.2s ease-in-out;
  cursor: pointer;
  color: white;
}
.menu:hover {
  color: cornflowerblue;
}

</style>