#!/bin/bash

# SignalR Testing Script
# This script tests all SignalR hubs by triggering events

API_URL="http://localhost:5162/api"

echo "🧪 SignalR Testing Script"
echo "========================"
echo ""

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}📌 Make sure you have:${NC}"
echo "  1. API running at http://localhost:5162"
echo "  2. Test client open at http://localhost:8080/test-all-signalr-hubs.html"
echo "  3. Clicked 'Test All Hubs Automatically' button"
echo ""
echo -e "${YELLOW}⏳ Waiting 5 seconds for you to set up...${NC}"
sleep 5

echo ""
echo "================================"
echo "🎯 Test 1: Booking Hub"
echo "================================"
echo ""
echo -e "${BLUE}ℹ️  Testing BookingCreated event...${NC}"
echo "Note: This might fail due to foreign key constraints if test users don't exist"
echo ""

# Create a booking (this will trigger BookingCreated event)
curl -X POST "$API_URL/bookings" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -d '{
    "userId": 1,
    "labId": 1,
    "zoneId": 1,
    "startTime": "2024-12-01T09:00:00",
    "endTime": "2024-12-01T11:00:00",
    "status": 1,
    "notes": "SignalR Test Booking"
  }' 2>/dev/null

echo ""
echo -e "${YELLOW}⏸️  Check your test client for 'BookingCreated' event${NC}"
echo "Press Enter to continue to next test..."
read

echo ""
echo "================================"
echo "🔧 Test 2: Equipment Hub"
echo "================================"
echo ""
echo -e "${BLUE}ℹ️  First, let's get an equipment ID...${NC}"

# Get equipment list
EQUIPMENT_RESPONSE=$(curl -s "$API_URL/equipment?PageNumber=1&PageSize=1")
echo "$EQUIPMENT_RESPONSE" | jq '.'

EQUIPMENT_ID=$(echo "$EQUIPMENT_RESPONSE" | jq -r '.items[0].equipmentId // empty')

if [ -z "$EQUIPMENT_ID" ]; then
  echo -e "${RED}❌ No equipment found in database${NC}"
else
  echo ""
  echo -e "${BLUE}ℹ️  Updating equipment $EQUIPMENT_ID to Broken status (2)...${NC}"
  echo "This should trigger EquipmentStatusChanged event"
  echo ""
  
  # Update equipment to broken status
  curl -X PUT "$API_URL/equipment/$EQUIPMENT_ID" \
    -H "Content-Type: application/json" \
    -d "{
      \"name\": \"Test Equipment (Updated)\",
      \"code\": \"EQ-TEST-001\",
      \"description\": \"Testing SignalR notification\",
      \"status\": 2,
      \"labId\": 1
    }" | jq '.'
  
  echo ""
  echo -e "${YELLOW}⏸️  Check your test client for 'EquipmentStatusChanged' event${NC}"
fi

echo "Press Enter to continue to next test..."
read

echo ""
echo "================================"
echo "🔒 Test 3: Security Log Hub"
echo "================================"
echo ""
echo -e "${BLUE}ℹ️  Creating a security log...${NC}"
echo "This should trigger SecurityAlert event"
echo ""

# Create security log
curl -X POST "$API_URL/securitylogs" \
  -H "Content-Type: application/json" \
  -d '{
    "eventId": 1,
    "securityId": 1,
    "actionType": "Unauthorized Access Attempt",
    "notes": "SignalR Test - Security alert triggered",
    "loggedAt": "'$(date -u +"%Y-%m-%dT%H:%M:%S")'"
  }' | jq '.'

echo ""
echo -e "${YELLOW}⏸️  Check your test client for 'SecurityAlert' event${NC}"
echo "Press Enter to continue to next test..."
read

echo ""
echo "================================"
echo "🎯 Test 4: Lab Event Hub"
echo "================================"
echo ""
echo -e "${BLUE}ℹ️  Creating a lab event...${NC}"
echo "This should trigger NewLabEvent event"
echo ""

# Create lab event
curl -X POST "$API_URL/labevents" \
  -H "Content-Type: application/json" \
  -d '{
    "labId": 1,
    "activityTypeId": 1,
    "title": "SignalR Test Event",
    "description": "Testing SignalR real-time notification",
    "startTime": "'$(date -u -d '+1 day' +"%Y-%m-%dT%H:%M:%S")'",
    "endTime": "'$(date -u -d '+1 day +2 hours' +"%Y-%m-%dT%H:%M:%S")'"
  }' | jq '.'

echo ""
echo -e "${YELLOW}⏸️  Check your test client for 'NewLabEvent' event${NC}"
echo "Press Enter to continue..."
read

echo ""
echo "================================"
echo "🔔 Test 5: Notification Hub"
echo "================================"
echo ""
echo -e "${YELLOW}⚠️  NotificationHub events are not yet implemented in controllers${NC}"
echo "This hub is ready but needs controller integration"

echo ""
echo "================================"
echo "✅ Testing Complete!"
echo "================================"
echo ""
echo -e "${GREEN}Summary:${NC}"
echo "  • Booking Hub: Tested (may fail if users don't exist)"
echo "  • Equipment Hub: Tested EquipmentStatusChanged event"
echo "  • Security Hub: Tested SecurityAlert event"
echo "  • Lab Event Hub: Tested NewLabEvent event"
echo "  • Notification Hub: Ready but not integrated yet"
echo ""
echo -e "${BLUE}📊 Check your browser test client to see all events received!${NC}"
echo ""
echo -e "${YELLOW}💡 Tips:${NC}"
echo "  • Check API terminal for detailed logs"
echo "  • Check browser console (F12) for SignalR debug logs"
echo "  • All events show in the test client's Event Log section"
echo ""
