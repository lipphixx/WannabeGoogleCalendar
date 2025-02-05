<script setup>
defineProps({
  day: Number,
  events: {
    type: Array,
    default: () => []
  },
  name: {
    type: String,
    default: null
  }
});

const emit = defineEmits(['addEvent', 'removeEvent']);

async function fetchJoke(day){
  // const url = new URL('https://api.chucknorris.io/jokes/random');
  //
  // if(category){
  //   url.searchParams.append('category', category);
  // }
  //
  // try {
  //   const response = await fetch(url.toString());
  //   const data = await response.json();
  //
  //   jokeDiv.innerHTML = data.value;
  // } catch (error) {
  //   jokeDiv.innerHTML = 'Při načítání došlo k chybě!';
  // }
}
</script>

<template>
  <div class="day" @click="emit('addEvent')">

    <section>
    <span class="day-number">{{ day }}</span>
    <span id="day-svatek" v-if="name">{{name}}</span>
    </section>
    <div v-if="events && events.length" class="events">
      <div
          v-for="(event, index) in events"
          :key="index"
          :class="[event.time ? 'timeEvent' : 'fDEvent']"
          @click.stop="emit('showEvent', event)"
      >
        <span>{{ event.name }}</span>
        <span v-if="event.time"> {{ event.time }}</span>
        <a href="#" @click.stop="emit('removeEvent', event.id)">❌</a>
      </div>
    </div>

  </div>
</template>

<style scoped>
.day {
  border: 1px solid #36363690;
  min-height: 111px;
  min-width: 170px;
  width: fit-content;
  position: relative;
  padding: 4px;
  cursor: pointer;
  color: white;
  border-radius: 3px;
}
.day-number {
  font-weight: bold;
}
.events {
  display: flex;
  flex-direction: column;
  width: max-content;
  margin-top: 5px;
  font-size: 12px;
  color: white;
}
.timeEvent {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: transparent;
  margin: 2px 0;
  padding: 2px;
  border-radius: 4px;
  border: solid 2px #3b82f6;
  width: 140px;
}

.fDEvent {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #3b82f6;
  margin: 2px 0;
  padding: 2px;
  border-radius: 4px;
  width: 140px;
}

section {
  display: flex;
  justify-content: space-between;
  margin-right: 5px;
}

#day-svatek {
  color: #FFFFFF90;
  font-size: 80%;
  text-align: right;
}

</style>
