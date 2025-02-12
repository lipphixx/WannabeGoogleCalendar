<script setup>
const props = defineProps(['loggedUser']);
const emit = defineEmits(['logOut']);

function logOut() {
  emit('logOut');
  window.location.reload();
}
</script>

<template>

  <section>
    <div class="row">
      <a class="material-symbols-outlined"> person </a>
      <p>{{ props.loggedUser }}</p>
    </div>
    <div class="row">
      <button class="tlacitko">Vytvořit událost</button>
    </div>
    <div class="row">
      <div id="checkboxContainer">
      <h3>Moje kalendáře</h3>
      <label>
        <input type="checkbox" name="" id="loggedUserCheck">
        {{ props.loggedUser }}
      </label>
      <label>
        <input type="checkbox" name="" id="holidayCheck">
        Státní svátky
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
</style>