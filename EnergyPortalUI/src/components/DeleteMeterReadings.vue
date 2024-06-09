<template>
  <div style="text-align: center;">
    <button class="button is-danger" :class="{ 'is-loading': isLoading }" @click="deleteMeterReadings" :disabled="isLoading">Delete Meter Readings</button>
    <div v-if="successMessage" class="notification is-success mt-4">
      {{ successMessage }}
    </div>
  </div>
</template>

<script>
import config from '../../config';
import axios from 'axios';

export default {
  name: 'DeleteMeterReadings',
  data() {
    return {
      isLoading: false,
      successMessage: ''
    };
  },
  methods: {
    async deleteMeterReadings() {
      try {
        this.isLoading = true;
        await axios.delete(config.baseUrl + '/api/meter-reading-uploads');
        this.successMessage = 'All Meter Reading Test Data removed'; 
        console.log('Meter readings deleted successfully');

        setTimeout(() => {
          this.successMessage = ''; 
        }, 3000);
      } catch (error) {
        console.error('Error deleting meter readings:', error);
      } finally {
        this.isLoading = false;
      }
    }
  }
};
</script>

<style scoped>
/* Add component-specific styles here if needed */
</style>
