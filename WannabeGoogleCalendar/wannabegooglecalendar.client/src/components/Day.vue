<script setup>
defineProps({
  day: Number,
  events: {
    type: Array,
    default: () => []
  },
  isToday: Boolean
});

const emit = defineEmits(['removeEvent']);
function removeEvent(index){
  emit('removeEvent', index);
}

</script>

<template>
<div id="hahaha" :class="{ today: isToday }">

  <h2>{{day}}</h2>

  <div v-if="events && events.length" id="meowmeowmeow">
    <div v-for="(event, index) in events" :key="index">
      <div v-if="event.time">
        {{ event.name }} {{ event.time }}
        <button @click.stop="removeEvent(index)" id="removeBtn">❌</button>
      </div>
      <div v-else>
        {{ event.name }} (celodenní)
        <button @click.stop="removeEvent(index)" id="removeBtn">❌</button>
      </div>
    </div>
  </div>

</div>
</template>

<style scoped>
div#hahaha {
  border: 1px solid black;
  width: 150px;
  height: 150px;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
}

h2 {
  margin-left: 5px;
}

#removeBtn {
  border: none;
  background: none;
}

#meowmeowmeow {
  border-radius: 2px;
  background: #00BFFFFF;
  color: white;
  max-height: 100%;
  display: flex;
  flex-wrap: wrap;
  border: none;
  overflow-y: auto;
}

#meowmeowmeow div {
  margin-left: 2px;
}

.today {
  background-color: #00BFFF20;
}
</style>