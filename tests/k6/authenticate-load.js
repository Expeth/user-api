import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 20 },
    { duration: '5m', target: 20 },
    { duration: '10s', target: 0 },
  ],
};

export default function () {
    const payload = JSON.stringify({
        login: 'kovetskiy',
        password: '1234567',
    });

    const params = {
        headers: {
          'Content-Type': 'application/json',
        },
      };
    

  const res = http.post('http://127.0.0.1:5000/users/authenticate', payload, params);
  check(res, { 'status was 200': (r) => r.status == 200 });
}