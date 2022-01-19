import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 10 },
    { duration: '5m', target: 10 },
    { duration: '10s', target: 0 },
  ],
};

export default function () {
    const payload = JSON.stringify({
        login: 'testuser1',
        password: '123456aF1!',
    });

    const params = {
        headers: {
          'Content-Type': 'application/json',
        },
      };
    

  const res = http.post('http://127.0.0.1:50000/users/authenticate', payload, params);
  check(res, { 'status was 200': (r) => r.status == 200 });
}