<script setup>
import {onMounted, ref, watch} from "vue";
import axios from "axios";
import PasswordReset from "@/components/passwordReset.vue";
import LittleCalendar from "@/components/littleCalendar.vue";

const userId = ref(null);
const userEmail = ref(null);
const userPhone = ref(null);
const userFullName = ref(null);
const onPasswordReset = ref(false);

const props = defineProps(['loggedUser', 'currentMonth', 'currentYear', 'createEvent', 'labelColors']);
const emit = defineEmits(['logOut', 'userEvents', 'holidayEvents', 'toggleLabel', 'chatbotToggle', 'participantEvents']);

const hideEvents = ref(false);
const hideHolidays = ref(false);
const hideParticipantEvents = ref(false);
const daysCurrent = ref([]);
const daysInCurrentMonth = ref(0);
const userSettings = ref(null);

const onEdit = ref(false);
const usernameInp = ref(null);
const emailInp = ref(null);
const fullNameInp = ref(null);
const phoneNumberInp = ref(null);

const errorValue = ref('');
const visibleLabels = ref({});

onMounted(async () => {
  errorValue.value = '';
  drawCalendar();

  const response = await axios.get('https://172.20.10.4:5050/api/Users');

  userEmail.value = response.data.find(x => x.username === props.loggedUser).email;
  userPhone.value = response.data.find(x => x.username === props.loggedUser).phoneNumber;
  userFullName.value = response.data.find(x => x.username === props.loggedUser).fullName;
  userId.value = response.data.find(x => x.username === props.loggedUser).userId;
});
watch([() => props.currentMonth, () => props.currentYear], () => {
  drawCalendar();
});

function drawCalendar() {
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
  if (hideEvents.value === false && hideParticipantEvents.value === false){
    console.log("true");
    hideEvents.value = true;
    hideParticipantEvents.value = true;

    emit('userEvents', hideEvents.value);
    emit('participantEvents', hideParticipantEvents.value);
  } else if (hideParticipantEvents.value === true && hideEvents.value === true) {
    hideParticipantEvents.value = false;
    hideEvents.value = false;
    emit('userEvents', hideEvents.value);
    emit('participantEvents', hideParticipantEvents.value);
  } else if (hideParticipantEvents.value === false && hideEvents.value === true) {
    hideEvents.value = false;
    emit('userEvents', hideEvents.value);
  } else {
    hideEvents.value = true;
    emit('userEvents', hideEvents.value);
  }

}

function toggleHolidayEvents() {
  hideHolidays.value = !hideHolidays.value
  emit('holidayEvents', hideHolidays.value)
}

function toggleParticipantEvents() {
  hideParticipantEvents.value = !hideParticipantEvents.value;
  emit('participantEvents', hideParticipantEvents.value);
}

function createEvent() {
  props.createEvent.action = {};
  props.createEvent.action = true;
}

function openChatbot() {
  emit('chatbotToggle', true);
}

function openUserSettings() {
  onEdit.value = false;
  onPasswordReset.value = false;
  userSettings.value.showModal();
}

function startEdit() {
  onEdit.value = true;
  errorValue.value = '';
  usernameInp.value = props.loggedUser;
  emailInp.value = userEmail.value;
  fullNameInp.value = userFullName.value;
  phoneNumberInp.value = userPhone.value;
}

async function saveEdit() {
  const credentials = {
    userId: userId.value,
    username: usernameInp.value,
    fullName: fullNameInp.value,
    email: emailInp.value,
    phoneNumber: String(phoneNumberInp.value)
  }

  try {
    await axios.post('https://172.20.10.4:5050/api/Auth/ChangeCredentials', credentials);
    if (confirm("Chcete opravdu pokračovat?")) {
      alert("Úspěšně uloženo! - Z bezpečnostních důvodů budete odhlášeni. ✅");
      errorValue.value = '';
      userSettings.value.close();
      sessionStorage.clear();
      window.location.reload();
    }
  } catch (error) {
    console.log(error);
    if (error.response) {
      switch (error.response.status) {
        case 400:
          errorValue.value = 'username';
          break;
        case 404:
          errorValue.value = 'phone';
          break;
        case 401:
          errorValue.value = 'email';
          break;
        default:
          errorValue.value = 'network';
      }
    } else {
      errorValue.value = 'network';
    }
  }
}

function openPasswordReset() {
  onPasswordReset.value = true;
  onEdit.value = false;
}

function toggleLabelFilter(label) {
  visibleLabels.value[label] = !visibleLabels.value[label];
  emit('toggleLabel', label);
}
</script>

<template>
  <section>
    <div>
      <dialog ref="userSettings" class="dialogs">
        <div v-if="!onEdit && !onPasswordReset" class="inputContainer">
          <h2 class="dialogRow inputStyle" style="font-weight: bold">Uživatelské nastavení</h2>
          <p class="dialogRow inputStyle">Uživatelské jméno: {{ props.loggedUser }}</p>
          <p class="dialogRow inputStyle">Jméno a příjmení: {{ userFullName }}</p>
          <p class="dialogRow inputStyle">Email: {{ userEmail }}</p>
          <p class="dialogRow inputStyle">Telefonní číslo: {{ userPhone }}</p>
          <div class="dialogRow" style="gap: 10px">
            <button @click="startEdit" v-if="!onEdit" class="editBtn">Upravit</button>
            <button @click="userSettings.close()" v-if="!onEdit" class="closeBtn">Zavřít</button>
          </div>
        </div>

        <div v-if="onEdit">
          <h2 class="dialogRow" style="font-weight: bold">Úprava profilu</h2>
          <label class="labelicek">
            Uživatelské jméno:
            <input class="dialogRow inputStyle" type="text" v-model="usernameInp" required>
          </label>
          <p v-if="errorValue === 'username'">Uživatelské jméno již existuje!</p>

          <label class="labelicek">
            Celé jméno:
            <input class="dialogRow inputStyle" type="text" v-model="fullNameInp" required>
          </label>

          <label class="labelicek">
            Email:
            <input class="dialogRow inputStyle" type="email" v-model="emailInp" required>
          </label>
          <p v-if="errorValue === 'email'">Email již existuje!</p>

          <label class="labelicek">
            Telefonní číslo:
            <input class="dialogRow inputStyle" type="tel" v-model="phoneNumberInp" required>
          </label>
          <p v-if="errorValue === 'phone'">Telefonní číslo již existuje!</p>

          <a class="bu" @click="openPasswordReset">Změnit heslo</a>
          <div class="dialogRow" style="gap: 10px">
            <button @click="saveEdit" class="addBtn">Uložit</button>
            <button @click="onEdit = false" class="closeBtn">Zrušit</button>
          </div>
        </div>

        <passwordReset v-if="onPasswordReset"
                       :email="userEmail"></passwordReset>
      </dialog>
    </div>
    <div id="flexicek">
      <div class="row" style="justify-content: space-between">
        <h1 style="font-weight: bold">@{{ props.loggedUser }}</h1>
        <a class="material-symbols-outlined person" @click="openUserSettings"> person </a>
      </div>
      <div class="row">
        <button class="addBtn" @click="createEvent">Vytvořit událost</button>
      </div>

      <div class="row" style="margin-left: 6%; margin-top: 15px">
        <littleCalendar>

        </littleCalendar>
      </div>

      <div class="row" style="margin-top: 2vh">
        <div id="checkboxContainer">
          <h3>Filtry</h3>
          <div class="boxicky">
            <div :style="{
          backgroundColor: hideEvents ? 'transparent' : '#6495ED',
          boxShadow: `inset 0 0 0 2px #6495ED`
        }"
                 class="checkboxLabels"
                 @click="toggleUserEvents()"
            >
            </div>
            {{ userFullName }}
          </div>
          <div class="boxicky">
            <div :style="{
          backgroundColor: hideParticipantEvents ? 'transparent' : '#6495ED',
          boxShadow: `inset 0 0 0 2px #6495ED`
        }"
                 class="checkboxLabels"
                 @click="toggleParticipantEvents()"
            >
            </div>
            Zúčastněné
          </div>
          <div class="boxicky">
            <div :style="{
          backgroundColor: hideHolidays ? 'transparent' : '#6495ED',
          boxShadow: `inset 0 0 0 2px #6495ED`
        }"
                 class="checkboxLabels"
                 @click="toggleHolidayEvents()"
            >
            </div>
            Svátky
          </div>
        </div>
      </div>
      <div class="row">
        <div id="checkboxContainer">
          <h3>Štítky</h3>
          <div style="display: flex; gap: 5px">
            <div class="checkboxik" v-for="(lC, label) in labelColors" :key="label">
              <div :style="{
          backgroundColor: visibleLabels[label] ? 'transparent': lC.color,
          boxShadow: `inset 0 0 0 2px ${lC.color}`
        }"
                   class="checkboxLabels"
                   @click="lC.visible !== false; toggleLabelFilter(label)"
              >
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
      <div id="bottom" style="font-size: 9px">
        <div style="display: flex; justify-content: space-between">
        <span class="material-symbols-outlined person" @click="openChatbot">chat</span>
        <span class="material-symbols-outlined person" @click="logOut">logout</span>
        </div>
        <div style="display: flex; justify-content: space-between; margin-top: 1vh">
          <a class="aCKO" href="https://policies.google.com/terms?hl=cs">Smluvní podmínky</a>
          <a class="aCKO" style="text-align: right" href="https://policies.google.com/privacy?hl=cs">Ochrana Soukromí</a>
        </div>
      </div>

  </section>
</template>

<style scoped>

a {
  justify-content: center;
}

.aCKO {
  text-decoration: none;
  color: #5e5e5e;
}

.aCKO:hover {
  text-decoration: underline;
}

.bu {
  cursor: pointer;
  font-size: 90%;
  color: cornflowerblue;
}

.bu:hover {
  text-decoration: underline;
}

section {
  display: flex;
  flex-direction: column;
  background: #1f1f1f;
  width: 14vw;
  height: 70vh;
}

.row, .lastRow {
  display: flex;
  align-items: center;
  width: 90%;
  color: white;
  margin-left: 5%;
  margin-right: 5%;
}

.dialogs {
  width: 400px;
  padding: 20px;
  border: none;
  border-radius: 8px;
  flex-direction: column;
  gap: 10px;
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background-color: #181818;
  color: white;
}

label {
  display: flex;
  flex-direction: column;
}

.inputContainer {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.dialogRow {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  height: 35px;
}

.inputStyle {
  width: 100%;
  padding: 8px;
  background-color: transparent;
  border: none;
  border-bottom: 1px solid #363636;
  color: white;
}

#flexicek {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-top: 4px;
}

.dialogRow:focus,
.dialogRow input:focus {
  outline: none;
  background: linear-gradient(to top, #36363620, #36363600);
}

.lastRow {
  color: cornflowerblue;
  cursor: pointer;
  margin-top: 10px;

}

.lastRow:hover {
  text-decoration: underline;
  cursor: pointer;
}

.boxicky {
  display: flex;
  flex-direction: row;
  gap: 5px;
  align-items: center;
}

#bottom {
  margin-top: auto;
  margin-bottom: 8vh;
  padding: 0 1vw 0 1vw;
}

.checkboxik {
  border-radius: 10px;
}

#checkboxContainer {
  display: flex;
  flex-direction: column;
}

button {
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
  border: none;
  padding: 5%;
  color: White;
  border-radius: 5px;
  width: 100%;
}

button:hover {
  background-position: right;
  cursor: pointer;
}

.logOut {
  border: 1px solid cornflowerblue;
  box-shadow: inset 0 0 0 1px cornflowerblue;
  background: linear-gradient(to right, #36363620, #36363600);
  transition: box-shadow 0.3s ease-in-out;
}

.logOut:hover {
  box-shadow: inset 0 0 2px 3px cornflowerblue;
}

.labelicek {
  display: flex;
  gap: 3px;
}

.noBackground {
  background-color: transparent;
}

.checkboxLabels {
  width: 20px;
  height: 20px;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.closeBtn {
  background: #36363650;
  border: 1px solid #36363690;
  color: White;
  cursor: pointer;
  text-align: center;
  font-size: 16px;
  border-radius: 5px;
  height: 35px;
  padding: 5px 10px;
  transition: border 0.3s ease-in-out, box-shadow 0.3s ease-in-out;
}

.closeBtn:hover {
  border: 2px solid cornflowerblue;
  box-shadow: inset 0 0 0 1px cornflowerblue;
}

.editBtn {
  border: 1px solid cornflowerblue;
  font-size: 16px;
  box-shadow: inset 0 0 0 1px cornflowerblue;
  background: linear-gradient(to right, #36363620, #36363600);
  transition: box-shadow 0.3s ease-in-out;
  height: 35px;
  padding: 5px 10px;
}

.editBtn:hover {
  box-shadow: inset 0 0 2px 3px cornflowerblue;
}

.addBtn {
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
  font-size: 16px;
  height: 35px;
  padding: 5px 10px;
}

.addBtn:hover {
  background-position: right;
}

.person {
  transition: color 0.2s ease-in-out;
  cursor: pointer;
}

.person:hover {
  color: cornflowerblue;
}
</style>